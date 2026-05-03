using Bitirme_Projesi.Pages;
namespace Bitirme_Projesi
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			Routing.RegisterRoute("Login", typeof(LoginPage));

			Routing.RegisterRoute("TimeLine", typeof(TimeLinePage));

			Routing.RegisterRoute("Register", typeof(RegisterPage));

			Routing.RegisterRoute("Quests", typeof(QuestsPage));

			Routing.RegisterRoute("Profile", typeof(ProfilePage));

			Routing.RegisterRoute("EditProfile", typeof(EditProfilePage));

			Routing.RegisterRoute("StartQuest", typeof(StartQuestPage));

			Routing.RegisterRoute("ComplateQuest", typeof(ComplateQuestPage));

			Routing.RegisterRoute("ShareQuest", typeof(ShareQuestPage));

			Routing.RegisterRoute("PostDetail", typeof(PostDetailPage));

			Routing.RegisterRoute("MyPosts", typeof(MyPostsPage));
		}
	}
}
