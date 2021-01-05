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
        public static readonly string CannotConvertInteger = "Cannot convert {0} to an integer.";
    }
}
