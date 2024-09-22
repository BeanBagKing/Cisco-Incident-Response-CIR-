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
    /// This PluginValidator implementation uses the public key token 
    /// of the host to compare it against the public key token of the assembly
    /// to validate.
    /// </summary>
    class KeyTokenPluginValidator : PluginValidator
    {
        private readonly byte[] hostPublicKeyToken;

        internal KeyTokenPluginValidator(Assembly theHost):base(theHost)
        {
            
            hostPublicKeyToken = Host.GetName().GetPublicKeyToken();
        }


        /// <summary>
        /// This method validates the passed assembly for compatibility 
        /// with the executing host.
        /// This method uses the public key token 
        /// of the host to compare it against the public key token of the assembly
        /// to validate.
        /// </summary>
        /// <param name="asm">the assembly to validate</param>
        /// <returns>true if the assembly is compatible, otherwise false</returns>
        public override bool Validate(Assembly asm)
        {
            if (asm == null) throw new ArgumentNullException("asm");
            byte[] hostPubKeyToken = hostPublicKeyToken;
            byte[] publicKeyToken = asm.GetName().GetPublicKeyToken();
            if (hostPubKeyToken.Length != publicKeyToken.Length)
                return false;
            for (uint i = 0; i < hostPubKeyToken.Length; i++)
            {
                if (hostPubKeyToken[i] != publicKeyToken[i])
                    return false;
            }
            return true;
        }
    }
}
