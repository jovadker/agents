// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AIBot.Bots
{
    public static class ActivityTypes
    {
        public const string Message = "message";
        public const string Trace = "trace";
    }

    public static class RoleTypes
    {
        public const int User = 1;
        public const int Bot = 0;
    }

    public static class BotDefaultMessages
    {
        public static string DYM = "To clarify, did you mean:";
        public static string NoneOfThese = "None of these";
    }

    public enum Interval
    {
        LastNTranscripts = 0,
        LastNDays = 1,
        LastNWeeks = 2
    }
}
