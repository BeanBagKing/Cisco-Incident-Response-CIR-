// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;

namespace Recurity.CIR.Engine.Helper
{
    public struct TriBool
    {
        // The three possible TriBool values:
        public static readonly TriBool UNKNOWN = new TriBool(0);
        public static readonly TriBool FALSE = new TriBool(-1);
        public static readonly TriBool TRUE = new TriBool(1);
        // Private field that stores -1, 0, 1 for FALSE, UNKNOWN, TRUE:
        private readonly int value;

        // Private constructor. The value parameter must be -1, 0, or 1:
        TriBool(int value)
        {
            this.value = value;
        }

        // Implicit conversion from bool to TriBool. Maps true to 
        // TriBool.TRUE and false to TriBool.FALSE:
        public static implicit operator TriBool(bool x)
        {
            return x ? TRUE : FALSE;
        }

        // Explicit conversion from TriBool to bool. Throws an 
        // exception if the given TriBool is UNKNOWN, otherwise returns
        // true or false:
        public static explicit operator bool(TriBool x)
        {
            if (x.value == 0) throw new InvalidOperationException();
            return x.value > 0;
        }

        // Equality operator. Returns UNKNOWN if either operand is UNKNOWN, 
        // otherwise returns TRUE or FALSE:
        public static TriBool operator ==(TriBool x, TriBool y)
        {
            if (x.value == 0 || y.value == 0) return UNKNOWN;
            return x.value == y.value ? TRUE : FALSE;
        }

        // Inequality operator. Returns UNKNOWN if either operand is
        // UNKNOWN, otherwise returns TRUE or FALSE:
        public static TriBool operator !=(TriBool x, TriBool y)
        {
            if (x.value == 0 || y.value == 0) return UNKNOWN;
            return x.value != y.value ? TRUE : FALSE;
        }

        // Logical negation operator. Returns TRUE if the operand is 
        // FALSE, UNKNOWN if the operand is UNKNOWN, or FALSE if the
        // operand is TRUE:
        public static TriBool operator !(TriBool x)
        {
            return new TriBool(-x.value);
        }

        // Logical AND operator. Returns FALSE if either operand is 
        // FALSE, UNKNOWN if either operand is UNKNOWN, otherwise TRUE:
        public static TriBool operator &(TriBool x, TriBool y)
        {
            return new TriBool(x.value < y.value ? x.value : y.value);
        }

        // Logical OR operator. Returns TRUE if either operand is 
        // TRUE, UNKNOWN if either operand is UNKNOWN, otherwise FALSE:
        public static TriBool operator |(TriBool x, TriBool y)
        {
            return new TriBool(x.value > y.value ? x.value : y.value);
        }

        // Definitely true operator. Returns true if the operand is 
        // TRUE, false otherwise:
        public static bool operator true(TriBool x)
        {
            return x.value > 0;
        }

        // Definitely false operator. Returns true if the operand is 
        // FALSE, false otherwise:
        public static bool operator false(TriBool x)
        {
            return x.value < 0;
        }

        // Overload the conversion from TriBool to string:
        public static implicit operator string(TriBool x)
        {
            return x.value > 0 ? "TRUE"
                 : x.value < 0 ? "FALSE"
                 : "UNKNOWN";
        }

        // Override the Object.Equals(object o) method:
        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (TriBool)o);
            }
            catch
            {
                return false;
            }
        }

        // Override the Object.GetHashCode() method:
        public override int GetHashCode()
        {
            return value;
        }

        // Override the ToString method to convert TriBool to a string:
        public override string ToString()
        {
            switch (value)
            {
                case -1:
                    return "false";
                case 0:
                    return "TriBool.UNKNOWN";
                case 1:
                    return "true";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
