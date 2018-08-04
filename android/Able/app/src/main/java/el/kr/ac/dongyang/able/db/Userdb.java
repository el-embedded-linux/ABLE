package el.kr.ac.dongyang.able.db;

import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;

import el.kr.ac.dongyang.able.model.UserModel;

public class Userdb {
    public UserModel userModel;
    public String userName;

    public Userdb() {
    }

    public String getUserName(String uid) {
        FirebaseDatabase.getInstance().getReference().child("USER").child(uid).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                userModel = dataSnapshot.getValue(UserModel.class);
                if (userModel != null) {
                    userName = userModel.getUserName();
                }
            }
            @Override
            public void onCancelled(DatabaseError databaseError) {
            }
        });
        return userName;
    }
}
