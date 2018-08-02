using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

    public GameObject bridge; 
    public GameObject enemy;
    public GameObject coin;

    Vector3 vector_enemy;
    Vector3 vector_coin;

    System.Random r = new System.Random(4807);


	// Use this for initialization
	void Start () {
        
        //①スタート時に，橋を建設する
        for (int i = 2 ; i <= 8; i++){
            if (i != 4){
                Instantiate(bridge, transform.position, transform.rotation);
            }
            transform.position += Vector3.forward * 60f;


        }

        //x : -8.4 ~ 8.4
        //y : 10.54
        //z : 12 ~ 1140
        //上記の範囲でランダムにコインとenemyを生成する

        //②enemyの生成
        for (int j = 0; j < 25; j++){


            //プレハブ化したものを使うべきであるが，
            //高さがうまくいっていないため，そのまま利用している
            vector_enemy = new Vector3((float)r.NextDouble() * 16.8f - 8.4f, enemy.gameObject.transform.position.y, r.Next(12, 200));
            Instantiate(enemy, vector_enemy, enemy.gameObject.transform.rotation);

            vector_coin = new Vector3((float)r.NextDouble() * 16.8f - 8.4f, coin.gameObject.transform.position.y, r.Next(12, 200));
            Instantiate(coin, vector_coin, coin.gameObject.transform.rotation);

            //Debug.Log("整数型：" + Random.Range(0, 3));
            //Debug.Log("小数型：" + Random.Range(0.0f, 3.0f));


        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
