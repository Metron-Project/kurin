#!/usr/bin/env python3
"""Tests for generate_models.py's classification/type-resolution/naming heuristics.

Run with: python3 -m unittest discover -s tools/codegen
(stdlib unittest -- no extra dependencies to install.)

These exercise the pure logic functions against small synthetic schemas rather than the real
"Metron Comicbook Database.yaml", so they stay fast and keep passing regardless of what the
live schema currently contains. They're here so the classification heuristics (e.g. "is this
enum's description semantic or just the number restated?") can be changed with confidence.
"""
from __future__ import annotations

import unittest

import generate_models as gm


class PascalCaseTests(unittest.TestCase):
    def test_snake_case(self):
        self.assertEqual(gm.pascal_case("cover_date"), "CoverDate")

    def test_single_word(self):
        self.assertEqual(gm.pascal_case("name"), "Name")

    def test_hyphen_and_underscore_mixed(self):
        self.assertEqual(gm.pascal_case("foo-bar_baz"), "FooBarBaz")

    def test_csharp_identifier_prefixes_leading_digit(self):
        self.assertEqual(gm.csharp_identifier("9_8"), "_98")


class ParseDescriptionNamesTests(unittest.TestCase):
    def test_semantic_names_are_extracted(self):
        description = (
            "* `1` - Cancelled\n"
            "* `2` - Completed\n"
            "* `3` - Hiatus\n"
            "* `4` - Ongoing\n"
        )
        names = gm.parse_description_names(description, [1, 2, 3, 4])
        self.assertEqual(names, {1: "Cancelled", 2: "Completed", 3: "Hiatus", 4: "Ongoing"})

    def test_value_restated_as_name_is_not_semantic(self):
        description = "* `1` - 1\n* `2` - 2\n"
        self.assertIsNone(gm.parse_description_names(description, [1, 2]))

    def test_missing_entry_returns_none(self):
        description = "* `1` - Cancelled\n"
        self.assertIsNone(gm.parse_description_names(description, [1, 2]))

    def test_empty_description_returns_none(self):
        self.assertIsNone(gm.parse_description_names("", [1]))

    def test_duplicate_names_return_none(self):
        description = "* `1` - Same Name\n* `2` - Same Name\n"
        self.assertIsNone(gm.parse_description_names(description, [1, 2]))


class ClassifySchemasTests(unittest.TestCase):
    def test_blank_and_null_enum_are_skipped(self):
        classification = gm.classify_schemas({
            "BlankEnum": {"enum": [""]},
            "NullEnum": {"enum": [None]},
        })
        self.assertEqual(classification["BlankEnum"].kind, gm.SchemaKind.SKIP)
        self.assertEqual(classification["NullEnum"].kind, gm.SchemaKind.SKIP)

    def test_paginated_wrapper_is_skipped(self):
        classification = gm.classify_schemas({
            "PaginatedArcListList": {
                "type": "object",
                "properties": {"count": {}, "next": {}, "previous": {}, "results": {}},
            },
        })
        self.assertEqual(classification["PaginatedArcListList"].kind, gm.SchemaKind.SKIP)

    def test_identifier_safe_string_enum(self):
        classification = gm.classify_schemas({
            "CurrencyEnum": {"type": "string", "enum": ["USD", "GBP"]},
        })
        self.assertEqual(classification["CurrencyEnum"].kind, gm.SchemaKind.STRING_ENUM)

    def test_string_enum_with_non_identifier_values_falls_back_to_scalar(self):
        classification = gm.classify_schemas({
            "WeirdEnum": {"type": "string", "enum": ["ok", "not ok"]},
        })
        classified = classification["WeirdEnum"]
        self.assertEqual(classified.kind, gm.SchemaKind.SCALAR_FALLBACK)
        self.assertEqual(classified.scalar_type, "string?")

    def test_integer_enum_with_semantic_names(self):
        classification = gm.classify_schemas({
            "StatusEnum": {
                "type": "integer",
                "enum": [1, 2],
                "description": "* `1` - Cancelled\n* `2` - Completed\n",
            },
        })
        classified = classification["StatusEnum"]
        self.assertEqual(classified.kind, gm.SchemaKind.INT_ENUM)
        self.assertEqual(classified.enum_names, {1: "Cancelled", 2: "Completed"})

    def test_integer_enum_without_semantic_names_falls_back_to_int(self):
        classification = gm.classify_schemas({
            "RatingEnum": {
                "type": "integer",
                "enum": [1, 2, 3],
                "description": "* `1` - 1\n* `2` - 2\n* `3` - 3\n",
            },
        })
        classified = classification["RatingEnum"]
        self.assertEqual(classified.kind, gm.SchemaKind.SCALAR_FALLBACK)
        self.assertEqual(classified.scalar_type, "int?")

    def test_number_enum_always_falls_back_to_decimal(self):
        # Decimal can't be a C# enum's underlying type, regardless of naming.
        classification = gm.classify_schemas({
            "DesiredGradeEnum": {
                "type": "number",
                "enum": [9.8, 9.9],
                "description": "* `9.8` - Near Mint\n* `9.9` - Mint\n",
            },
        })
        classified = classification["DesiredGradeEnum"]
        self.assertEqual(classified.kind, gm.SchemaKind.SCALAR_FALLBACK)
        self.assertEqual(classified.scalar_type, "decimal?")

    def test_plain_object_schema_is_a_class(self):
        classification = gm.classify_schemas({
            "Arc": {"type": "object", "properties": {"id": {"type": "integer"}}},
        })
        self.assertEqual(classification["Arc"].kind, gm.SchemaKind.CLASS)


class CSharpTypeForTests(unittest.TestCase):
    def setUp(self):
        self.classification = gm.classify_schemas({
            "Arc": {"type": "object", "properties": {}},
            "StatusEnum": {
                "type": "integer", "enum": [1],
                "description": "* `1` - Cancelled\n",
            },
            "RatingEnum": {
                "type": "integer", "enum": [1],
                "description": "* `1` - 1\n",
            },
            "BlankEnum": {"enum": [""]},
            "NullEnum": {"enum": [None]},
        })

    def test_ref_to_class(self):
        cs_type, converter = gm.csharp_type_for({"$ref": "#/components/schemas/Arc"}, self.classification)
        self.assertEqual(cs_type, "Arc?")
        self.assertIsNone(converter)

    def test_ref_to_int_enum(self):
        cs_type, _ = gm.csharp_type_for({"$ref": "#/components/schemas/StatusEnum"}, self.classification)
        self.assertEqual(cs_type, "StatusEnum?")

    def test_ref_to_scalar_fallback_enum_uses_scalar_type_directly(self):
        cs_type, _ = gm.csharp_type_for({"$ref": "#/components/schemas/RatingEnum"}, self.classification)
        self.assertEqual(cs_type, "int?")

    def test_allOf_single_ref_resolves_through(self):
        schema = {"allOf": [{"$ref": "#/components/schemas/Arc"}], "readOnly": True}
        cs_type, _ = gm.csharp_type_for(schema, self.classification)
        self.assertEqual(cs_type, "Arc?")

    def test_oneOf_with_null_enum_picks_the_real_ref(self):
        schema = {"oneOf": [
            {"$ref": "#/components/schemas/StatusEnum"},
            {"$ref": "#/components/schemas/NullEnum"},
        ]}
        cs_type, _ = gm.csharp_type_for(schema, self.classification)
        self.assertEqual(cs_type, "StatusEnum?")

    def test_oneOf_with_only_blank_or_null_falls_back_to_string(self):
        schema = {"oneOf": [
            {"$ref": "#/components/schemas/BlankEnum"},
            {"$ref": "#/components/schemas/NullEnum"},
        ]}
        cs_type, _ = gm.csharp_type_for(schema, self.classification)
        self.assertEqual(cs_type, "string?")

    def test_array_of_integers(self):
        schema = {"type": "array", "items": {"type": "integer"}}
        cs_type, _ = gm.csharp_type_for(schema, self.classification)
        self.assertEqual(cs_type, "List<int>?")

    def test_array_of_refs_strips_trailing_nullable(self):
        schema = {"type": "array", "items": {"$ref": "#/components/schemas/Arc"}}
        cs_type, _ = gm.csharp_type_for(schema, self.classification)
        self.assertEqual(cs_type, "List<Arc>?")

    def test_string_date_format(self):
        cs_type, _ = gm.csharp_type_for({"type": "string", "format": "date"}, self.classification)
        self.assertEqual(cs_type, "DateOnly?")

    def test_string_date_time_format(self):
        cs_type, _ = gm.csharp_type_for({"type": "string", "format": "date-time"}, self.classification)
        self.assertEqual(cs_type, "DateTimeOffset?")

    def test_string_decimal_format_uses_converter(self):
        cs_type, converter = gm.csharp_type_for({"type": "string", "format": "decimal"}, self.classification)
        self.assertEqual(cs_type, "decimal?")
        self.assertEqual(converter, "DecimalStringConverter")

    def test_number_decimal_format_uses_converter(self):
        cs_type, converter = gm.csharp_type_for({"type": "number", "format": "decimal"}, self.classification)
        self.assertEqual(cs_type, "decimal?")
        self.assertEqual(converter, "DecimalStringConverter")

    def test_plain_number_without_format(self):
        cs_type, converter = gm.csharp_type_for({"type": "number"}, self.classification)
        self.assertEqual(cs_type, "double?")
        self.assertIsNone(converter)

    def test_boolean(self):
        cs_type, _ = gm.csharp_type_for({"type": "boolean"}, self.classification)
        self.assertEqual(cs_type, "bool?")

    def test_freeform_object(self):
        cs_type, _ = gm.csharp_type_for({"type": "object"}, self.classification)
        self.assertEqual(cs_type, "Dictionary<string, object>?")


class FilterClassNameTests(unittest.TestCase):
    def test_simple_list_operation(self):
        self.assertEqual(gm.filter_class_name("api_issue_list"), "IssueFilter")

    def test_nested_sub_resource_list_operation(self):
        self.assertEqual(gm.filter_class_name("api_arc_issue_list_list"), "ArcIssueListFilter")

    def test_multi_word_resource(self):
        self.assertEqual(gm.filter_class_name("api_pull_list_issues_list"), "PullListIssuesFilter")


class RenderSmokeTests(unittest.TestCase):
    """Loose smoke tests that rendering produces the expected shape, not exact-text assertions."""

    def test_render_class_includes_json_property_name_and_type(self):
        classification = gm.classify_schemas({
            "Arc": {
                "type": "object",
                "properties": {"name": {"type": "string", "maxLength": 255}},
            },
        })
        output = gm.render_class("Arc", {"properties": {"name": {"type": "string"}}}, classification)
        self.assertIn("public sealed class Arc", output)
        self.assertIn('[JsonPropertyName("name")]', output)
        self.assertIn("public string? Name { get; set; }", output)

    def test_render_filter_class_includes_query_parameter_attribute(self):
        params = [{"name": "cv_id", "schema": {"type": "integer"}, "description": "Comic Vine ID"}]
        output = gm.render_filter_class("ArcFilter", params)
        self.assertIn("public sealed class ArcFilter", output)
        self.assertIn('[QueryParameter("cv_id")]', output)
        self.assertIn("public int? CvId { get; set; }", output)


if __name__ == "__main__":
    unittest.main()
