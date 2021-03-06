﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Base;
using ClownFish.HttpServer.Utils;

namespace ClownFish.HttpServer.Config
{
	/// <summary>
	/// 表示整个服务实例的运行参数
	/// </summary>
	[Serializable]
	public sealed class ServerOption
	{
		/// <summary>
		/// HTTP监听参数
		/// </summary>
		[XmlArray("httpListener")]
		[XmlArrayItem("option")]
		public HttpListenerOption[] HttpListenerOptions { get; set; }

        /// <summary>
        /// 通用配置参数
        /// </summary>
        [XmlArray("appSettings")]
        [XmlArrayItem("add")]
        public AppSetting[] AppSettings { get; set; }


        /// <summary>
        /// 需要加载的模块（类型字符串）
        /// </summary>
        [XmlArray("modules")]
		[XmlArrayItem("type")]
		public string[] Modules { get; set; }


		internal Type[] ModuleTypes { get; set; }
		internal Type[] HandlerTypes { get; set; }

		/// <summary>
		/// 需要加载的处理器工厂（类型字符串）
		/// </summary>
		[XmlArray("handlers")]
		[XmlArrayItem("type")]
		public string[] Handlers { get; set; }
		/// <summary>
		/// 站点参数
		/// </summary>
		[XmlElement("website")]
		public WebsiteOption Website { get; set; }

        /// <summary>
        /// 根据ServerOption转变的一些内部参数，主要是为了提升性能的一些缓存数据
        /// </summary>
        [NonSerialized]
        internal InternalOptions InternalOptions;   // { get; set; }
    }

    /// <summary>
    /// key/value 配置项
    /// </summary>
    [Serializable]
    public sealed class AppSetting
    {
        /// <summary>
        /// key
        /// </summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <summary>
        /// value
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

	/// <summary>
	/// HTTP监听参数
	/// </summary>
	[Serializable]
	public sealed class HttpListenerOption
	{
		/// <summary>
		/// 协议，可选范围：http, https
		/// </summary>
		[XmlAttribute("protocol")]
		public string Protocol { get; set; }
		/// <summary>
		/// 需要监听的IP地址
		/// </summary>
		[XmlAttribute("ip")]
		[DefaultValue("*")]
		public string Ip { get; set; }
		/// <summary>
		/// 需要监听的TCP端口
		/// </summary>
		[XmlAttribute("port")]
		public int Port { get; set; }

		/// <summary>
		/// 重写ToString方法
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Protocol}://{Ip}:{Port}/";
		}

        /// <summary>
        /// 显式成URL格式
        /// </summary>
        /// <returns></returns>
        public string ToUrl()
        {
            if( this.Ip == "*" ) {
                //string host = System.Net.Dns.GetHostName();
                string host = EnvironmentHelper.GetComputerName();
                return $"{Protocol}://{host}:{Port}/";

                //if( string.IsNullOrEmpty(host) == false )
                //    return $"{Protocol}://{host}:{Port}/";
                //else
                //    return $"{Protocol}://localhost:{Port}/";
            }
            else
                return $"{Protocol}://{Ip}:{Port}/";
        }
    }

	/// <summary>
	/// 站点参数
	/// </summary>
	[Serializable]
	public sealed class WebsiteOption
	{
		/// <summary>
		/// 站点的本地路径
		/// </summary>
		[XmlElement("localPath")]
		public string LocalPath { get; set; }

        /// <summary>
        /// 静态文件的默认缓存时间。
        /// 如果配置值小于等于零，则表示缓存时间为一年。
        /// </summary>
        [DefaultValue(0)]
        public int CacheDuration { get; set; }

        /// <summary>
        /// 静态文件参数
        /// </summary>
        [XmlArray("staticFile")]
		[XmlArrayItem("option")]
		public StaticFileOption[] StaticFiles { get; set; }

        /// <summary>
        /// 目录浏览相关参数
        /// </summary>
        [XmlElement("directoryBrowse")]
        public DirectoryBrowseOption DirectoryBrowse { get; set; }

    }

	/// <summary>
	/// 静态文件参数
	/// </summary>
	[Serializable]
	public sealed class StaticFileOption
	{
		/// <summary>
		/// 扩展名
		/// </summary>
		[XmlAttribute("ext")]
		public string Ext { get; set; }
		/// <summary>
		/// 需要缓存的秒数， 
		/// 小于 0  表示不缓存，
		/// 等于 0  表示取默认值（当前版本为 1年）
		/// 大于 0  表示缓存秒数
		/// </summary>
		[XmlAttribute("cache")]
		[DefaultValue(0)]
		public int Cache { get; set; }
		/// <summary>
		/// 扩展名对应的MimeType
		/// </summary>
		[XmlAttribute("mime")]
		public string Mine { get; set; }
	}

    /// <summary>
    /// 目录浏览相关参数
    /// </summary>
    [Serializable]
    public sealed class DirectoryBrowseOption
    {
        /// <summary>
        /// 默认文件名，例如：index.html，
        /// 如果有多个文件名，用分号分开。
        /// </summary>
        [XmlElement("defaultFile")]
        public string DefaultFile { get; set; }
    }


    /// <summary>
    /// 根据ServerOption转变的一些内部参数，主要是为了提升性能的一些缓存数据
    /// </summary>
    internal class InternalOptions
    {
        public Dictionary<string, int> StaticFileExtNames { get; set; }


        public string[] DefaultFiles { get; set; }
    }
}
