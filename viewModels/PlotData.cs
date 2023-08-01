namespace Lside_Mixture.ViewModels
{
    public class PlotData<T>
    {
        public PlotData(string title, T[] data)
        {
            this.Title = title;
            this.Data = data;
        }

        public string Title { get; set; }

        public T[] Data { get; set; }
    }
}
