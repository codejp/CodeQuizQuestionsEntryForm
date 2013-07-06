using Owin;

namespace CodeQuizQuestionsEntryForm
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
