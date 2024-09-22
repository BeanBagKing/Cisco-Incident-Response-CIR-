//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.Reflection;

namespace PluginHost.PluginValidation
{
    /// <summary>
    /// PluginValidator provides a simple method to validate an assembly for compatibility.
    /// Subclasses of PluginValidator implement several algorithm to check the passed assemblies 
    /// for validity, plausibility and compatibility.
    /// </summary>
    public abstract class PluginValidator
    {
        private readonly Assembly host;
        private readonly Version hostVersion;

        protected PluginValidator(Assembly theHost)
        {
            if (theHost == null) throw new ArgumentNullException("theHost");
            host = theHost;
            hostVersion = host.GetName().Version;
        }

        /// <summary>
        /// The host assembly
        /// </summary>
        protected Assembly Host
        {
            get { return host; }
        }

        public static PluginValidator Validator(Assembly hostAssembly)
        {
            if (hostAssembly == null) throw new ArgumentNullException("hostAssembly");

            // we can replace this if required
            return new KeyTokenPluginValidator(hostAssembly);
        }

        /// <summary>
        /// This method validates the passed assembly for compatibility 
        /// with the executing host.
        /// The algorithm used depends on the implementation.
        /// </summary>
        /// <param name="asm">the assembly to validate</param>
        /// <returns>true if the assembly is compatible, otherwise false</returns>
        public abstract bool Validate(Assembly asm);

        /// <summary>
        /// Compares the Major - Version of the assembly with the host assembly.
        /// 
        /// </summary>
        /// <param name="asm"></param>
        /// <returns>true if the Major-Version matches.</returns>
        public virtual bool ValidateVersion(Assembly asm)
        {
            Version asmVersion = asm.GetName().Version;
            return hostVersion.Major == asmVersion.Major;
        }
    }
}