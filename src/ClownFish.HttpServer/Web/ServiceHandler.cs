﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.HttpServer.Common;
using ClownFish.HttpServer.Result;

namespace ClownFish.HttpServer.Web
{
	/// <summary>
	/// 实现IHttpHandler接口，用于执行某个服务方法
	/// </summary>
	public sealed class ServiceHandler : IHttpHandler2
	{
		private Type _serviceType;
		private MethodInfo _method;

		internal ServiceHandler(Type serviceType, MethodInfo method)
		{
			_serviceType = serviceType;
			_method = method;
		}

		/// <summary>
		/// 处理HTTP请求的入口方法
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 处理HTTP请求的入口方法
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public IActionResult ProcessRequest2(HttpContext context)
		{
			// 创建类型实例
			object instance = _serviceType.FastNew();

			// 构造方法的调用参数
			ParameterResolver pr = ObjectFactory.New<ParameterResolver>();
			object[] parameters = pr.GetParameters(_method, context.Request);

			// 执行方法
			object result = null;
			if( _method.HasReturn() )
				result = _method.FastInvoke(instance, parameters);
			else
				_method.FastInvoke(instance, parameters);

			// 没有执行结果，直接返回（不产生输出）
			if( result == null )
				return null;


			// 转换结果
			IActionResult actionResult = result as IActionResult;
			if( actionResult == null ) {
				ResultConvert converter = ObjectFactory.New<ResultConvert>();
				actionResult = converter.Convert(result, context);
			}

			return actionResult;
		}
	}
}