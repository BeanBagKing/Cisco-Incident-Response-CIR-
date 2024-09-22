// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Recurity.CIR.Engine.Helper
{
    public class Singleton<T>
        where T : class
    {
        /// <summary>
        /// Gets the singleton instance of the type.
        /// </summary>
        public static T Instance
        {
            get { return SingletonProvider<T>.Instance; }
        }

        /// <summary>
        /// Protected constructor to prevent users from directly instantiating
        /// a Singleton object.
        /// </summary>
        protected Singleton()
        {
        }

        static class SingletonProvider<TYPE>
              where TYPE : class
        {
            private static TYPE SingletonInstance = null;

            public static TYPE Instance
            {
                get
                {
                    lock (typeof(SingletonProvider<TYPE>))
                    {
                        if (SingletonInstance == null)
                        {
                            try
                            {
                                SingletonInstance = typeof(TYPE).InvokeMember(typeof(TYPE).Name,
                                    BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
                                    null, null, null) as TYPE;
                            }
                            catch (TargetInvocationException Error)
                            {
                                /* we want to throw the exception that caused the TargetInvocationException */
                                throw Error.InnerException;
                            }
                        }
                    }

                    return SingletonInstance;
                }
            }

            static SingletonProvider()
            {
            }
        }
    }
}
