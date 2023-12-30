namespace WebClient.Pages
{
    public partial class Home
    {
        public string Text { get; set; } = "????";
        public string ButtonText { get; set; } = "Click Me";
        public int ButtonClicked { get; set; }

        void ButtonOnClick()
        {
            ButtonClicked += 1;
            Text = $"Awesome x {ButtonClicked}";
            ButtonText = "Click Me Again";
        }
    }
}
