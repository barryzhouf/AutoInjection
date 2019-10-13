# AutoInjection

һ��.net core����ע���AOP�����������

��������������
��װAutoInjection NuGet����
asp.net core Startup.cs�ļ�


	public void ConfigureServices(IServiceCollection services)
	{
		//�������ע�����AutoInjection
		services.AddMvc().AddAutowired();

		//����һ���ӿ�ע�뷽ʽ
		services.AddSbSingleton<Iuserdao, userdao>();
		services.AddSbSingleton<Iuserservice, userservice>();

		//����������ע�뷽ʽ
		//services.AddSbSingleton<userdao>();
		//services.AddSbSingleton<userservice>();

		//AOP������
		services.AddSbSingleton<TestInterceptor>();
	}

Controllerʹ�÷�ʽ��

	//asp.net mvc������ʹ������ע��
	public class HomeController : Controller
	{
		//����ע���ǩ
		[Autowired]
        public Iuserservice uservice;

        public IActionResult Index()
        {
            var list = uservice.getList();
            return View();
        }
	}

��ͨclass��ʹ�÷�ʽ

	//��ͨclass��ʹ������ע��
	public class userservice : Iuserservice
    {
        [Autowired]
        private Iuserdao udao { set; get; }

        public List<string> getList()
        {
            return udao.getList();
        }
    }

AOP����ʵ����

	public class TestInterceptor : TransactionalInterceptor
	{
		/// <summary>
		/// ִ��ǰ
		/// </summary>
		/// <param name="invocation"></param>
		public override void BeforeExecute(IInvocation invocation)
		{
			Console.WriteLine("{0}����ǰ����������", invocation.Method.Name, invocation.Arguments.Length);     
		}

		/// <summary>
		/// ִ�к�
		/// </summary>
		/// <param name="invocation"></param>
		public override void Executed(IInvocation invocation)
		{
			Console.WriteLine("{0}���غ󣬷���ֵ��{1}", invocation.Method.Name, invocation.ReturnValue);
		}
	}

�ӿ���AOP���ʾ��

	public interface Iuserservice
    {
		//Transactional AOP���ر�ǩ��TestInterceptor AOP���ش����ࡣ
        [Transactional(typeof(TestInterceptor))]
        List<string> getList();
    }
