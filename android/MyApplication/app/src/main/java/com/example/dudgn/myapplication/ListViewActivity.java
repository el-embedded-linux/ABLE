package com.example.dudgn.myapplication;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.ListView;

public class ListViewActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_list_view);

        //리스트뷰의 아이템
        String[] list_item = {"대연","영훈","지수","수현"};

        //리스트뷰 맵핑
        ListView listView = findViewById(R.id.listview);

        //리스트뷰의 어댑터 세팅
        ArrayAdapter arrayAdapter = new ArrayAdapter(this,android.R.layout.simple_list_item_1,list_item);

        //리스트뷰에 어댑터 연결
        listView.setAdapter(arrayAdapter);
    }
}
