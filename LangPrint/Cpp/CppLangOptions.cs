﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangPrint.Cpp
{
    public class CppLangOptions : LangOptions
    {
        public bool GeneratePackageSyntax { get; init; } = false;
        public int VariableMemberTypePadSize { get; init; } = 0;
        public int InlineCommentPadSize { get; init; } = 0;
    }
}
