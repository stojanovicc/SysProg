using System.Collections.Generic;

public class MuseumResponse
{
    public List<Museum> data { get; set; }

    public MuseumResponse()
    {
        data = new List<Museum>();
    }
}