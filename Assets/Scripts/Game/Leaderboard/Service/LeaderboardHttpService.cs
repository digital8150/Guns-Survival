using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // Newtonsoft.Json ���ӽ����̽� �߰�

public class LeaderboardHttpService
{
    private readonly string _baseUrl;
    private readonly MonoBehaviour _coroutineRunner;

    public LeaderboardHttpService(string baseUrl, MonoBehaviour coroutineRunner)
    {
        _baseUrl = baseUrl;
        _coroutineRunner = coroutineRunner;
    }

    // Top 10 ��ŷ ��������
    public void GetTop10(Action<List<ScoreEntry>> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(GetRequest<List<ScoreEntry>>($"{_baseUrl}/leaderboard/top10", onSuccess, onError));
    }

    // ���� ���
    public void AddScore(ScoreCreate score, Action<ScoreEntry> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(PostRequest($"{_baseUrl}/leaderboard", score, onSuccess, onError));
    }

    // Ư�� ID �ֺ� ��ŷ ��������
    public void GetAroundId(string scoreId, Action<List<ScoreEntry>> onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(GetRequest<List<ScoreEntry>>($"{_baseUrl}/leaderboard/{scoreId}/around", onSuccess, onError));
    }

    // Ư�� ���� ����
    public void DeleteScore(string scoreId, Action onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(DeleteRequest($"{_baseUrl}/leaderboard/{scoreId}", onSuccess, onError));
    }

    // �������� �ʱ�ȭ
    public void ResetLeaderboard(Action onSuccess, Action<string> onError)
    {
        _coroutineRunner.StartCoroutine(DeleteRequest($"{_baseUrl}/leaderboard", onSuccess, onError));
    }

    private IEnumerator GetRequest<T>(string url, Action<T> onSuccess, Action<string> onError)
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
                    // JsonUtility ��� JsonConvert.DeserializeObject ���
                    // ���� JSON �迭�� ���� List<T>�� ��ȯ ����
                    T result = JsonConvert.DeserializeObject<T>(json);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}\nReceived JSON: {json}");
                }
            }
        }
    }

    private IEnumerator PostRequest<T>(string url, object payload, Action<T> onSuccess, Action<string> onError)
    {
        // JsonUtility ��� JsonConvert.SerializeObject ����Ͽ� ���̷ε� ����ȭ
        string jsonPayload = JsonConvert.SerializeObject(payload);
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
                string json = request.downloadHandler.text;
                try
                {
                    // JsonUtility ��� JsonConvert.DeserializeObject ���
                    var result = JsonConvert.DeserializeObject<T>(json);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}\nReceived JSON: {json}");
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