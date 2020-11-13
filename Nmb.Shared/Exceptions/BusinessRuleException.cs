using System;
using System.Collections.Generic;
using System.Linq;

namespace Nmb.Shared.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRule Rule { get; private set; }

        public BusinessRuleException(int violatedRuleCode) : base(BusinessRule.GetErrorMessage(violatedRuleCode))
        {
            Rule = BusinessRule.GetRule(violatedRuleCode);
        }

        public BusinessRuleException(BusinessRule rule) : base(rule.Message)
        {
            Rule = rule;
        }
    }

    public class BusinessRule
    {
        public static List<BusinessRule> All = new List<BusinessRule>();

        public static BusinessRule DeleteAssociatedMedia = RegisterRule(10001, @"Media item is currently in used and cannot be deleted.");
        public static BusinessRule LearningModuleNotFound = RegisterRule(10002, @"Learning module not found");
        public static BusinessRule KnowledgeAttachmentNotFound = RegisterRule(10003, @"KnowledgeAttachmentNotFound");
        public static BusinessRule CapsuleAttachmentNotFound = RegisterRule(10004, @"CapsuleAttachmentNotFound");
        public static BusinessRule LearningRecordNotFound = RegisterRule(10005, @"LearningRecordNotFound");
        public static BusinessRule KnowledgeModuleNotFound = RegisterRule(10006, @"KnowledgeModuleNotFound");
        public static BusinessRule GroupNotFound = RegisterRule(10007, @"GroupNotFound");
        public static BusinessRule LearningStateNotFound = RegisterRule(10008, @"LearningStateNotFound");
        public static BusinessRule InvalidCommentOperation = RegisterRule(10009, @"InvalidCommentOperation");
        public static BusinessRule InvalidSocialOperation = RegisterRule(10010, @"InvalidSocialOperation");
        public static BusinessRule NoCommentFound = RegisterRule(10011, @"NoCommentFound");
        public static BusinessRule ItemNotFound = RegisterRule(10012, @"ItemNotFound");
        public static BusinessRule NoThreadFound = RegisterRule(10013, @"NoThreadFound");
        public static BusinessRule NotificationNotFound = RegisterRule(10014, @"NotificationNotFound");
        public static BusinessRule UserExist = RegisterRule(10015, @"UserExist");
        public static BusinessRule AdminExist = RegisterRule(10016, @"AdminExist");
        public static BusinessRule CreateUserError = RegisterRule(10017, @"CreateUserError");
        public static BusinessRule RoleNotFound = RegisterRule(10018, @"RoleNotFound");
        public static BusinessRule UserNotFound = RegisterRule(10019, @"UserNotFound");
        public static BusinessRule RoleExist = RegisterRule(10020, @"RoleExist");
        public static BusinessRule CapsuleHasBeenRated = RegisterRule(10021, @"CapsuleHasBeenRated");
        
        public static BusinessRule RegisterRule(int code, string message)
        {
            var rule = new BusinessRule(code, message);
            All.Add(rule);
            return rule;
        }

        public static string GetErrorMessage(int code)
        {
            return All.FirstOrDefault(t => t.Code == code)?.Message;
        }

        public static BusinessRule GetRule(int code)
        {
            return All.FirstOrDefault(t => t.Code == code);
        }


        public int Code { get; private set; }
        public string Message { get; private set; }

        public BusinessRule(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
