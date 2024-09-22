// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using Recurity.CIR.Engine.Helper;

namespace Recurity.CIR.Engine.Report
{
    public class ReportFormatter : Singleton<ReportFormatter>
    {
        private const string HEX_FORMAT = "0x{0}";
        private ReportFormatter()
        {
        }
        
        public String IntToHex(int value)
        {
            return String.Format(HEX_FORMAT, value.ToString("X"));
        }

        public String UIntToHex(uint value)
        {
           return String.Format(HEX_FORMAT, value.ToString("X"));
        }

        public String LongToHex(long value)
        {
           return String.Format(HEX_FORMAT, value.ToString("X"));
        }

        public String ULongToHex(ulong value)
        {
          return String.Format(HEX_FORMAT, value.ToString("X"));
        }


        public String formatAnker(String value)
        {
            return (value != null? value.Replace('.', '_'): "null") + "anker";
        }
    }
}
