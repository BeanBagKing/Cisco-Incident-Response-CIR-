//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser.Utils
{
    internal class Pair<T,V>
    {
        private readonly T head;
        private readonly V tail;

        public Pair(T head, V tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public V Tail
        {
            get { return tail; }
        }

        public T Head
        {
            get { return head; }
        }

       
    }
}