﻿using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class NamingPatternsParser : INamingPatternsParser
{
        
    private readonly DelimitedTextParser _keywordsParser = new('{','}', '\\');

    public List<ApplicationPatternPart> ParseApplicationPatterns(string pattern)
    {
        return ParseApplicationPatternsInternal(pattern).ToList();
    }

    private IEnumerable<ApplicationPatternPart> ParseApplicationPatternsInternal(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new EmptyPatternException();

        foreach (var (text, isDelimited) in _keywordsParser.Parse(pattern))
        {
            if (isDelimited)
            {
                if (!Enum.TryParse<ApplicationKeyword>(text, true, out var dynamicTextBaseType))
                {
                    var allowedKeywords = Enum.GetValues<ApplicationKeyword>().Select(type =>
                        _keywordsParser.StartDelimiter + type.ToString() + _keywordsParser.EndDelimiter);

                    throw new KeywordUnknownException(text, allowedKeywords);
                }

                yield return new DynamicTextApplicationPatternPart(dynamicTextBaseType);
            }
            else
            {
                yield return new StaticTextApplicationPatternPart(text);
            }
        }
    }

}