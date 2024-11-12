var tokenFirebase = '';

var firebaseConfig = {
    apiKey: "AIzaSyDh9IsH3JOdQCBnKxCqHIqa3BIIApOSZBk",
    authDomain: "appsesc-c5437.firebaseapp.com",
    databaseURL: 'https://appsesc-c5437.firebaseio.com',
    projectId: "appsesc-c5437",
    storageBucket: "appsesc-c5437.appspot.com",
    messagingSenderId: "1053997836471",
    appId: "1:1053997836471:web:a35864127df5546df68532",
    measurementId: "G-1Q4JGQPNT6"
};
// Initialize Firebase
firebase.initializeApp(firebaseConfig);
//firebase.analytics();

const messaging = firebase.messaging();
messaging.requestPermission()
    .then(function () {
        return messaging.getToken();
    })
    .then(function (token) {
        var teste = '';
        teste = token;

        $.ajax({
            url: '/cliente/token-firebase',
            data: {token:token},
            type: "POST",
            success: function (data) {
                //do stuff with json result 
                if (data.code === 1) {
                    console.log("ok");
                }
                
            },
            error: function (passParams) {
                console.log("Error is " + passParams);
            }
        });

        //console.log(teste);
    })
    .catch(function (err) {
     //   console.log('erro');
    })

messaging.onMessage(function (payload) {
   // console.log('Message: ', payload);
});