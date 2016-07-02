﻿/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.Msie.Configuration;
using JavaScriptEngineSwitcher.V8;
using React.TinyIoC;

namespace React
{
	/// <summary>
	/// Handles registration of core ReactJS.NET components.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Gets the IoC container. Try to avoid using this and always use constructor injection.
		/// This should only be used at the root level of an object heirarchy.
		/// </summary>
		public static TinyIoCContainer Container
		{
			get { return TinyIoCContainer.Current; }
		}

		/// <summary>
		/// Registers standard components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			// One instance shared for the whole app
			container.Register<IReactSiteConfiguration>((c, o) => ReactSiteConfiguration.Configuration);
			container.Register<IFileCacheHash, FileCacheHash>().AsPerRequestSingleton();
			container.Register<IJavaScriptEngineFactory, JavaScriptEngineFactory>().AsSingleton();

			container.Register<IReactEnvironment, ReactEnvironment>().AsPerRequestSingleton();
			RegisterSupportedEngines(container);
		}

		/// <summary>
		/// Registers JavaScript engines that may be able to run in the current environment
		/// </summary>
		/// <param name="container"></param>
		private void RegisterSupportedEngines(TinyIoCContainer container)
		{
			if (JavaScriptEngineUtils.EnvironmentSupportsClearScript())
			{
				container.Register(new JavaScriptEngineFactory.Registration
				{
					Factory = () => new V8JsEngine(),
					Priority = 10
				}, "ClearScriptV8");
			}
			if (JavaScriptEngineUtils.EnvironmentSupportsVroomJs())
			{
				container.Register(new JavaScriptEngineFactory.Registration
				{
					Factory = () => new VroomJsEngine(),
					Priority = 10
				}, "VroomJs");
			}

			container.Register(new JavaScriptEngineFactory.Registration
			{
				Factory = () => new MsieJsEngine(new MsieConfiguration { EngineMode = JsEngineMode.ChakraEdgeJsRt }),
				Priority = 20
			}, "MsieChakraEdgeRT");
			container.Register(new JavaScriptEngineFactory.Registration
			{
				Factory = () => new MsieJsEngine(new MsieConfiguration { EngineMode = JsEngineMode.ChakraIeJsRt }),
				Priority = 30
			}, "MsieChakraIeRT");
			container.Register(new JavaScriptEngineFactory.Registration
			{
				Factory = () => new MsieJsEngine(new MsieConfiguration { EngineMode = JsEngineMode.ChakraActiveScript }),
				Priority = 40
			}, "MsieChakraActiveScript");
		}
	}
}
