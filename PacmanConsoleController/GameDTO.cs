namespace PacmanConsoleController;

public class GameDTO
{
    public int gameId {  get; set; }
    public int mapId { get; set; }
    public bool isRunning { get; set; }
    public bool hasEnded { get; set; }
    public int playersInGame { get; set; }
}
