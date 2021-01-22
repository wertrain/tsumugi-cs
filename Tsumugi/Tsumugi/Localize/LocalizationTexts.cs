using System;
using System.Collections.Generic;
using System.Text;

namespace Tsumugi.Localize
{
    public class LocalizationTexts
    {
        public static readonly string CannotFindJumpTarget = "Cannot find label {0} to jump to.";
        public static readonly string NotDefined = "{0} is not defined.";
        public static readonly string AlreadyUsedLabelName = "{0} has already been used as the label name.";
        public static readonly string CannotFindAttributeRequiredTag = "Cannot find attribute {0} which is required for tag {1}. ";

        public static readonly string ThisTokenMustBe = "In this token, it must be {0}, not {1}.";
        public static readonly string CannotConvertNumber = "Cannot convert {0} to an {1}.";
        public static readonly string NoAssociatedWith = "There is no {1} associated with {0}.";
        public static readonly string UnknownOperatorPrefix = "Unknown operator : {0}{1}";
        public static readonly string UnknownOperatorInfix = "Unknown operator : {0} {1} {2}";
        public static readonly string TypeMismatch = "Type mismatch : {0} {1} {2}";
        public static readonly string UndefinedIdentifier = "Undefined identifier {0}.";
        public static readonly string NotFunction = "{0} is not a function.";
        public static readonly string NumberOfArgumentsDoesNotMatch = "Number of arguments to the function {0} does not match.";
        public static readonly string DoesNotSupportArgumentsOfType = "{0} does not support arguments of type {1}.";
        public static readonly string AssigningValuesNotAllowed = "Assigning values to other than identifiers is not allowed.";
        public static readonly string NoValueFoundForAttribute = "No value found for attribute {1} in {0} tag.";
        public static readonly string SyntaxError = "Syntax error.";
        public static readonly string ErrorInStructureIfTag = "Error in the structure of the if tag.";

        public static readonly string LexingPosition = "Lines: {0} Columns: {1} Position: {2}";
    }
}
