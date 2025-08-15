using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardHttpService
{
    private readonly string _baseUrl;
    private readonly MonoBehaviour _coroutineRunner;

    public LeaderboardHttpService(string baseUrl, MonoBehaviour coroutineRunner)
    {
        _baseUrl = baseUrl;
        _coroutineRunner = coroutineRunner;
    }

    // Top 10 랭킹 가져오기
    public void GetTop10(Action<List<ScoreEntry>> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(GetRequest<List<ScoreEntry>>($"{_baseUrl}/leaderboard/top10", onSuccess, onError, true));
    }

    // 점수 등록
    public void AddScore(ScoreCreate score, Action<ScoreEntry> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(PostRequest($"{_baseUrl}/leaderboard", score, onSuccess, onError));
    }

    // 특정 ID 주변 랭킹 가져오기
    public void GetAroundId(string scoreId, Action<List<ScoreEntry>> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(GetRequest<List<ScoreEntry>>($"{_baseUrl}/leaderboard/{scoreId}/around", onSuccess, onError, true));
    }

    // 특정 점수 삭제
    public void DeleteScore(string scoreId, Action onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(DeleteRequest($"{_baseUrl}/leaderboard/{scoreId}", onSuccess, onError));
    }

    // 리더보드 초기화
    public void ResetLeaderboard(Action onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(DeleteRequest($"{_baseUrl}/leaderboard", onSuccess, onError));
    }


    private IEnumerator GetRequest<T>(string url, Action<T> onSuccess, Action<string> onError, bool isList = false)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Error [{request.responseCode}]: {request.error}");
            }
            else
            {
                string json = request.downloadHandler.text;
                try
                {
                    T result;
                    if (isList)
                    {
                        string wrappedJson = $"{{\"items\":{json}}}";
                        var wrapper = JsonUtility.FromJson<ScoreEntryListWrapper>(wrappedJson);
                        result = (T)(object)wrapper.items;
                    }
                    else
                    {
                        result = JsonUtility.FromJson<T>(json);
                    }
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}");
                }
            }
        }
    }

    private IEnumerator PostRequest<T>(string url, object payload, Action<T> onSuccess, Action<string> onError)
    {
        string jsonPayload = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Error [{request.responseCode}]: {request.error}");
            }
            else
            {
                try
                {
                    var result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}");
                }
            }
        }
    }

    private IEnumerator DeleteRequest(string url, Action onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Error [{request.responseCode}]: {request.error}");
            }
            else
            {
                onSuccess?.Invoke();
            }
        }
    }
}