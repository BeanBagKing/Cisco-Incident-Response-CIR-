//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

namespace Recurity.CIR.Engine.Executor
{
    public static class ExecutorFactory
    {

        public static IEngineExecuter SingleThreadedExecutor()
        {
            return new EngineExecuter();
            
        }

        public static  IEngineExecuter ThreadedExecutor()
        {
            return new ThreadedEngineExecutor();
        }
    }
}
