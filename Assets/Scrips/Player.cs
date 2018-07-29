using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    int coinCount = 0;
    bool mode = false;

    public Image gage;
    public Image bakusoku;

    public Image syucyu;
    public Image fade;

    Quaternion ini_gyro;
    Quaternion gyro;



    void Awake(){

    }

    // Use this for initialization
    void Start () {
        bakusoku.gameObject.SetActive(false);
        syucyu.gameObject.SetActive(false);
        fade.gameObject.SetActive(false);

        Input.gyro.enabled = false;

        ini_gyro = Input.gyro.attitude;

	}
	
	// Update is called once per frame
	void Update () {
        
        //①自動でまっすぐすすむ(チェックフォームより)
        if(!mode){
            this.transform.Translate(Vector3.forward * 1.0f);
        }else{
            //2倍速
            this.transform.Translate(Vector3.forward * 1.0f * 2.0f);
        }


        //②左右に動く処理(ジャイロをとったら，メソッドひとつ)
        if(!Input.gyro.enabled){
            if (Input.GetKey(KeyCode.RightArrow)){
                moveRight();
            }else if (Input.GetKey(KeyCode.LeftArrow)){
                moveLeft();
            }
        }


        //②' ジャイロ実装

        if (Input.gyro.enabled){
            gyro = Input.gyro.attitude;
            //Debug.Log("x:" + gyro.x);
            //Debug.Log("y:" + gyro.y);
            Debug.Log("z:" + gyro.z);

            //①ジャイロの初期値を取得する(絶対値変換)
            //②それよりもどれくらいずれているを考える

            if (ini_gyro.z < 0) ini_gyro.z = -(ini_gyro.z);
            if (gyro.z < 0) gyro.z = -(gyro.z);

            float difference = gyro.z - ini_gyro.z;

            //Debug.Log("difference:" + difference);

            int dif = Mathf.RoundToInt(difference);

            //Debug.Log("dif:" + dif);

            //③取得したzの値は絶対値で考える


            if ((this.transform.position.x >= -9.0f) && (this.transform.position.x <= 9.0f)){

                this.transform.Translate(Vector3.right * ini_gyro.z * 1.2f);
            }

    
        }

        //③シーン遷移
        //ゴールしたら遷移，ちょっと過ぎてからゴールにした
        if(this.transform.position.z > 843f){
            SceneManager.LoadScene("Goal");
        }

        if(coinCount == 10){
            bakusoku.gameObject.SetActive(true);
        }

	}

    public void onClick(){
        //coin==10でコルーチンに飛ばす,画像を見えないようにしたい
        if (coinCount == 10){
            StartCoroutine("Bakusoku");
        }
    }

    void moveRight(){
        if(this.transform.position.x <= 9.0f){
            this.transform.Translate(Vector3.right * 0.3f);
        }
    }

    void moveLeft(){
        if (this.transform.position.x >= -9.0f) {
            this.transform.Translate(Vector3.left * 0.3f);
        }
    }


    void OnTriggerStay(Collider other){
        //モードのON/OFFによって変わる。非同期処理か。
        //enemyタグにぶつかったら画面遷移
        if (other.gameObject.CompareTag("Enemy")){
            //if文でモードにより変更！

            if(!mode){
                //いい感じに上の飛ばせない。。。ビルソンにひっかかってた
                //this.transform.Translate(Vector3.up * 5.0f);

                this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 100, 0);

                StartCoroutine("Fade");
                //fail(); 
            }else if(mode){
                // ビルソンを飛ばす処理
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-8.4f, 8.4f), 100, 0);
            }


        } 
    }


    void OnTriggerEnter(Collider other){
        //coinにあたったら+1
        if(other.gameObject.CompareTag("Coin")){
            if(coinCount < 10) {
                coinCount++;
                gage.fillAmount += 0.1f;
                Debug.Log("coinCount:" + coinCount);
            }

            Destroy(other.gameObject);
        }
    }

    // コルーチン  
    private IEnumerator Bakusoku() {
        //コインが10になったら，爆速モード変更
        mode = true;
        syucyu.gameObject.SetActive(true);

        // 10秒待つ  
        yield return new WaitForSeconds (5.0f);

        mode = false;
        coinCount = 0;
        gage.fillAmount = 0;
        bakusoku.gameObject.SetActive(false);
        syucyu.gameObject.SetActive(false);

    }  

    private IEnumerator Fade(){

        fade.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        Vector3 vector3 = new Vector3(-0.13f, 11.26f, -25.72f);
        this.gameObject.transform.position = vector3; //TODO 強引にスタート位置に戻るようにした。パーカーに相談

        SceneManager.LoadScene("Main");

        fade.gameObject.SetActive(false);


    }

    void fail(){
        fade.gameObject.SetActive(true);

        Vector3 vector3 = new Vector3(-0.13f, 11.26f, -25.72f);
        this.gameObject.transform.position = vector3; //スタート位置に戻る

        SceneManager.LoadScene("Main");


        fade.gameObject.SetActive(false);

    }



}

