using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ScoreCreate
{
    public int score { get; set; }
    public int survival_time { get; set; }
    public string nickname { get; set; }

    public ScoreCreate(int score, int survivalTime, string nickname)
    {
        this.score = score;
        this.survival_time = survivalTime;
        this.nickname = nickname;
    }
}


// API ���信 ����ϴ� ������
[Serializable]
public class ScoreEntry
{
    public string id { get; set; }
    public DateTime registration_date { get; set; }
    public int score { get; set; }
    public int survival_time { get; set; }
    public string nickname { get; set; }

    public override string ToString()
    {
        return $"{nickname} - Score: {score}, Survival Time: {survival_time}, Registered: {registration_date}";
    }
}

[Serializable]
class ScoreEntryListWrapper
{
    public List<ScoreEntry> items;
}