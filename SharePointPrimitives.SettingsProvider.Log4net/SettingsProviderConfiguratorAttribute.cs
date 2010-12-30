#region BSD license
// Copyright 2010 Chris Dibbs. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY Chris Dibbs ``AS IS'' AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Chris Dibbs OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Chris Dibbs.
#endregion

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace SharePointPrimitives.SettingsProvider.Log4net {

    /// <summary>
    /// Assembly level attribute to configure the <see cref="XmlConfigurator"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute may only be used at the assembly scope and can only
    /// be used once per assembly.
    /// </para>
    /// <para>
    /// Use this attribute to configure the <see cref="XmlConfigurator"/>
    /// without calling one of the <see cref="XmlConfigurator.Configure()"/>
    /// methods.
    /// </para>
    /// <note>
    /// <para>
    /// Log4net will only look for assembly level configuration attributes once.
    /// When using the log4net assembly level attributes to control the configuration 
    /// of log4net you must ensure that the first call to any of the 
    /// <see cref="log4net.Core.LoggerManager"/> methods is made from the assembly with the configuration
    /// attributes. 
    /// </para>
    /// <para>
    /// If you cannot guarantee the order in which log4net calls will be made from 
    /// different assemblies you must use programmatic configuration instead, i.e.
    /// call the <see cref="XmlConfigurator.Configure()"/> method directly.
    /// </para>
    /// </note>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    [AttributeUsage(AttributeTargets.Assembly), Serializable]
    public sealed class SettingsProviderConfiguratorAttribute : ConfiguratorAttribute {
        
        public SettingsProviderConfiguratorAttribute()
            : base(0) /* configurator priority 0 */
        { }

        /// <summary>
        /// Configures the <see cref="ILoggerRepository"/> for the specified assembly.
        /// </summary>
        /// <param name="sourceAssembly">The assembly that this attribute was defined on.</param>
        /// <param name="targetRepository">The repository to configure.</param>
        /// <remarks>
        /// <para>
        /// Configure the repository using the <see cref="XmlConfigurator"/>.
        /// The <paramref name="targetRepository"/> specified must extend the <see cref="Hierarchy"/>
        /// class otherwise the <see cref="XmlConfigurator"/> will not be able to
        /// configure it.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="targetRepository" /> does not extend <see cref="Hierarchy"/>.</exception>
        override public void Configure(Assembly sourceAssembly, ILoggerRepository targetRepository) {
            Stream config = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Settings.Default.Log4NetConfiguration));
            XmlConfigurator.Configure(targetRepository, config);
        }
    }
}