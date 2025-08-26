using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightedRandom<T> //가중치 랜덤 뽑기
{
    private readonly Dictionary<T, double> items = new();
    private double totalWeight;

    public void Add(T item, double weight) //아이템 추가
    {
        //가중치는 양수만
        if(weight <= 0) throw new System.ArgumentException("Weight must be Positive");
        //아이템 중복방지
        if(items.ContainsKey(item)) throw new System.ArgumentException("Item already exists");
        
        items[item] = weight; //딕셔너리에 아이템과 가중치 저장
        totalWeight += weight; //전체 가중치 합계
    }

    public T GetRandom()
    {
        double randomWeight = UnityEngine.Random.value * totalWeight;
        double cumulativeWeight = 0;

        foreach (var kvp in items)
        {
            cumulativeWeight += kvp.Value; //아이템 가중치 더함
            if (randomWeight <= cumulativeWeight)
            {
                double percent = (kvp.Value / totalWeight) * 100;
                Debug.Log($"{kvp.Key} 뽑힘! 확률: {percent:F2}%");
                return kvp.Key;
            }
        }

        return items.Last().Key; //마지막 아이템 반환
    }
}
