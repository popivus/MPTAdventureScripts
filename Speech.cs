public class Speech
{
    public Speech(string speech, string name, float time)
    {
        this.speech = speech;
        this.name = name;
        this.time = time;
    }

    public string speech {get; set;}
    public string name {get; set;}
    public float time {get; set;}
}