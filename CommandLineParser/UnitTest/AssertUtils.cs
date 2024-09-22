//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System.Collections;
using NUnit.Framework;

namespace Recurity.CommandLineParser.UnitTest
{
    public class AssertUtils
    {

        public static void AssertContains(ICollection collection, object o)
        {
            if (collection == null) throw new AssertionException("collection must not be null");
            if (o == null) throw new AssertionException("object must not be null");

            foreach (object o1 in collection)
            {
                
                if(o1 != null && o1.Equals(o))
                    return;
            }
            throw new AssertionException(string.Format("Object {0} not found in collection",o));

        }
    }
}
