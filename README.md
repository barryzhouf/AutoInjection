# AutoInjection

一款.net core依赖注入和AOP拦截器组件。

引入该组件方法：
安装AutoInjection NuGet包。
asp.net core Startup.cs文件


	public void ConfigureServices(IServiceCollection services)
	{
		//添加依赖注入组件AutoInjection
		services.AddMvc().AddAutowired();

		//方法一：接口注入方式
		services.AddSbSingleton<Iuserdao, userdao>();
		services.AddSbSingleton<Iuserservice, userservice>();

		//方法二：类注入方式
		//services.AddSbSingleton<userdao>();
		//services.AddSbSingleton<userservice>();

		//AOP拦截类
		services.AddSbSingleton<TestInterceptor>();
	}

Controller使用方式：

	//asp.net mvc控制器使用依赖注入
	public class HomeController : Controller
	{
		//依赖注入标签
		[Autowired]
        public Iuserservice uservice;

        public IActionResult Index()
        {
            var list = uservice.getList();
            return View();
        }
	}

普通class类使用方式

	//普通class类使用依赖注入
	public class userservice : Iuserservice
    {
        [Autowired]
        private Iuserdao udao { set; get; }

        public List<string> getList()
        {
            return udao.getList();
        }
    }

AOP拦截实现类

	public class TestInterceptor : TransactionalInterceptor
	{
		/// <summary>
		/// 执行前
		/// </summary>
		/// <param name="invocation"></param>
		public override void BeforeExecute(IInvocation invocation)
		{
			Console.WriteLine("{0}拦截前，参数个数", invocation.Method.Name, invocation.Arguments.Length);     
		}

		/// <summary>
		/// 执行后
		/// </summary>
		/// <param name="invocation"></param>
		public override void Executed(IInvocation invocation)
		{
			Console.WriteLine("{0}拦截后，返回值是{1}", invocation.Method.Name, invocation.ReturnValue);
		}
	}

接口类AOP标记示例

	public interface Iuserservice
    {
		//Transactional AOP拦截标签，TestInterceptor AOP拦截处理类。
        [Transactional(typeof(TestInterceptor))]
        List<string> getList();
    }
