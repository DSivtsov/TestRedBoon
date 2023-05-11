using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public enum AmmountFormat
    {
        USD = 1,
    }
    public class AmmountFormatter
    {
        public static string GetAmmount(int ammount)
        {
            switch (FormatSettings.CurrentAmmountFormat)
            {
                case AmmountFormat.USD:
                    return $"$ {ammount:### ##0}"; ;
                default:
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    if (FormatSettings.CurrentAmmountFormat != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                        throw new NotImplementedException($"Not Implemented AmmountFormat [{FormatSettings.CurrentAmmountFormat}]");
                    else
                        throw new NotImplementedException("AmmountFormat not set");
            }
        }
    }

    public class FormatSettings
    {
        public static AmmountFormat CurrentAmmountFormat { get; } = AmmountFormat.USD;
    }
}
