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

    // 総フレーム数
    int GENOM_LENGTH = 210;
    // 遺伝子情報の大きさ
    int MAX_GENOM_LIST = 10;

    // 遺伝子10個分を一括管理
    List<List<int>> genomList = new List<List<int>>();
    //List<int> coinList = new List<int>();
    List<int> frameList = new List<int>(); // 進んだフレーム数をMAX_GENOM_LIST分保持する

    int GENERATIONS = 20; // 20回繰り返したら終わりとする
    int geneCount = 0; // 世代数

    int tryCount = 0;   // トライ回数
    int frameCount = 0; // 現在のフレーム数

    void Awake(){

        Debug.Log("Awake");

        for (int i = 0; i < MAX_GENOM_LIST;i++){
            List<int> list = new List<int>();
            for (int j = 0; j < GENOM_LENGTH;j++){
                // 遺伝子作成
                // 0 or 1をList<int>に代入
                list.Add(UnityEngine.Random.Range(0, 2));
            }
            genomList.Add(list);

            // coinListの初期化
            // coinList.Add(0);

            // frameListの初期化(10個全てに0を代入)
            frameList.Add(0);
        }

        // お作法の練習

        //int index = 0;

        //foreach (List<int> list in genomList){
        //    foreach(int i in list){
        //        Debug.Log(index + ":" + i);
        //    }
        //    index++;
        //}

    }
    // gaメソッド
    // * tryCountが10になったら評価(実行される)
    // * GENERATIONS = 20になったら評価終了

    // TODO
    // // frameListに現在のフレーム数を保持->そのあと0に初期化
    // geneListに則って，車を動かす
    // // tryCount周り

    void ga(){

        // もしゴールしたら，そのゴールしたリストを出力する．

        // 個体の評価に関しては，ゴールしたかどうか=>frameCountの進み具合
        // (コイン数によって決める(なお，途中で死んだ場合はcoin=0))

        // 選択
        // 初期化処理
        List<int> top2 = new List<int>(){0,0};      // 要素数2つ
        List<int> N_top2 = new List<int>(){0,0};    // 要素数2つ

        // ここでのtop2はフレーム数の最大値
        // 上位2個体を探す(準備)
        // 便宜上 top2[0]>top2[1]とする
        if (frameList[0] > frameList[1]){
            top2[0] = frameList[0];
            top2[1] = frameList[1];
            N_top2[0] = 0;
            N_top2[1] = 1;
        }else{
            top2[0] = frameList[1];
            top2[1] = frameList[0];
            N_top2[0] = 1;
            N_top2[1] = 0;
        }

        for (int i = 2; i < MAX_GENOM_LIST; i++){
            // 上位1個体を探す
            if (frameList[i] > top2[0]){
                top2[1] = top2[0];      //2番目に大きい値にする
                top2[0] = frameList[i]; //最も大きくする
                N_top2[0] = i;
                N_top2[1] = 0;
            } else if (frameList[i] > top2[1]) {
                top2[1] = frameList[i];
                N_top2[1] = i;
            }
        }

        // 表示確認
        Debug.Log("選択完了");

        // エリート配列を確保する
        List<List<int>> elite = new List<List<int>>();

        for (int i = 0; i < 2; i++){
            elite.Add(genomList[N_top2[i]]);
        }

        // Max世代数いくとここで打ち切り
        if (geneCount == GENERATIONS - 1){
            Debug.Log("打ち切り");
            return;
        }

        // ルーレット選択

        // 次の世代の入れ物をつくる

        List<List<int>> nextE = new List<List<int>>();

        for (int i = 0; i < MAX_GENOM_LIST; i++){
            List<int> list = new List<int>();
            for (int j = 0; j < GENOM_LENGTH; j++){
                list.Add(0);
            }
            nextE.Add(list); // 一旦{0,0,0,0,0,0,....}を10個持つリストを生成する．
        }

        // 的
        int total = 0;
        for (int i = 0; i < MAX_GENOM_LIST; i++){
            total += frameList[i];
        }

        for (int x = 0; x < MAX_GENOM_LIST; x++){
            int arrow = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * total);
            int sum = 0;

            for (int i = 0; i < MAX_GENOM_LIST; i++){
                sum += frameList[i];
                if (sum > arrow){
                    nextE[x] = genomList[i];
                    break;
                }
            }
        }

        // 交叉(前から2組ずつ交叉する。交叉しなければそのままコピーされる)
        // ２組ずつ選ぶので偶奇で場合分けする。
        for (int x = 0; (MAX_GENOM_LIST % 2 == 1 && x < MAX_GENOM_LIST - 1) || (MAX_GENOM_LIST % 2 == 0 && x < MAX_GENOM_LIST); x = x + 2){
            
            // crossrate % で交叉
            int crossrate = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * 100);
            if (crossrate < 95){
                
                // 2点交叉でcopyEからchildをつくる
                // r1~r2までの染色体を入れ替える
                int r1 = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * GENOM_LENGTH);
                int r2 = r1 + Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * (GENOM_LENGTH - r1));
                List<List<int>> child = new List<List<int>>();
                // child初期化
                for (int i = 0; i < 2; i++){
                    List<int> list = new List<int>();
                    for (int j = 0; j < GENOM_LENGTH; j++){
                        list.Add(0);
                    }
                    child.Add(list);
                }

                for (int i = 0; i < GENOM_LENGTH; i++){
                    if (r1 <= i && i <= r2){
                        child[0][i] = nextE[x + 1][i];
                        child[1][i] = nextE[x][i];
                    }else{
                        child[0][i] = nextE[x][i];
                        child[1][i] = nextE[x + 1][i];
                    }
                }

                // childを世代に入れる
                nextE[x] = child[0];
                nextE[x + 1] = child[1];
            } else {
                Debug.Log("入れ替えなし");
            }

            // 突然変異
            // 各個体r%の確率で染色体のどこかを反転させる
            for (int i = 0; i < MAX_GENOM_LIST; i++){
                
                int mutantrate = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * 100);
                if (mutantrate < 3){
                    int m = Mathf.FloorToInt(UnityEngine.Random.Range(0.0f, 1.0f) * GENOM_LENGTH);
                    nextE[i][m] = (nextE[i][m] + 1) % 2;
                }
            }
        }

        // 仮個体を次の世代個体に代入
        for (int i = 0; i < MAX_GENOM_LIST; i++){
            if (i == 0 || i == 1){
                genomList[i] = elite[i];
            } else {
                genomList[i] = nextE[i];
            }
        }


        Debug.Log("世代数" + geneCount);
        geneCount++;

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

        frameCount++; // 進んだ距離

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
            //Debug.Log(frameCount);

            //if (genomList[tryCount][frameCount] == 0){
            //    moveRight();
            //}else if (genomList[tryCount][frameCount] == 1){
            //    moveLeft();
            //}

            //Debug.Log(count);
            //count ++;
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

            //float difference = gyro.z - ini_gyro.z;

            //Debug.Log("difference:" + difference);

            //int dif = Mathf.RoundToInt(difference);

            //Debug.Log("dif:" + dif);

            //③取得したzの値は絶対値で考える


            if ((this.transform.position.x >= -9.0f) && (this.transform.position.x <= 9.0f)){

                this.transform.Translate(Vector3.right * ini_gyro.z * 1.2f);
            }

        }

        //③シーン遷移
        //ゴールしたら遷移，ちょっと過ぎてからゴールにした
        if(this.transform.position.z > 183f){
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

                //StartCoroutine("Fade");

                Fade();


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
                coinCount+=10;
                gage.fillAmount += 0.1f;
                //Debug.Log("coinCount:" + coinCount);
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

    //private IEnumerator Fade(){
        
    //    fade.gameObject.SetActive(true);

    //    yield return new WaitForSeconds(0.5f);

    //    Vector3 vector3 = new Vector3(-0.13f, 11.26f, -25.72f);
    //    this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    //    this.gameObject.transform.position = vector3; //TODO 強引にスタート位置に戻るようにした。パーカーに相談

    //    tryCount++;
    //    Debug.Log("衝突：" + tryCount);

    //    frameList[tryCount] = frameCount;
    //    frameCount = 0;

    //    //SceneManager.LoadScene("Main");

    //    fade.gameObject.SetActive(false);


    //}

    private void Fade()
    {

        fade.gameObject.SetActive(true);

        //yield return new WaitForSeconds(0.5f);

        Vector3 vector3 = new Vector3(-0.13f, 11.26f, -25.72f);
        this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        this.gameObject.transform.position = vector3; //TODO 強引にスタート位置に戻るようにした。パーカーに相談

        frameList[tryCount] = frameCount;
        frameCount = 0;

        Debug.Log("衝突：" + tryCount);
        tryCount++;

        if (tryCount == 10){
            ga();
            tryCount = 0;
        }

        //SceneManager.LoadScene("Main");

        //coinCount;
        fade.gameObject.SetActive(false);


    }



    //void fail(){
    //    fade.gameObject.SetActive(true);

    //    Vector3 vector3 = new Vector3(-0.13f, 11.26f, -25.72f);
    //    this.gameObject.transform.position = vector3; //スタート位置に戻る

    //    SceneManager.LoadScene("Main");


    //    fade.gameObject.SetActive(false);

    //}

    // count = 10 になったらgaメソッドが呼ばれる
    //void setCoin(){
    //    coinList[count] = coinCount;
    //}

}

