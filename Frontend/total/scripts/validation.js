
const getDomElements = function(){
  console.log("dom geladen");
};

const uitlezendata = () => {
	handleData('.json', processdata);
};

const fetchData = async function(url, method = "GET", body = null) {
  return fetch(`https://mctproject2.azurewebsites.net/api/v1/${url}`, {
    method: method,
    body: body,
    headers: { "content-type": "application/json" }
  })
    .then(r => r.json())
    .then(data => data);
};

const buttonclick = function(){
  let btn = document.querySelector('.js-sign-in-button');
  btn.addEventListener('click', function()
  {
    let mailvalue = document.getElementById('username').value;
    let ww = document.getElementById('password').value;
   
   
    getAPI(`login/${mailvalue}/${ww}`)
  })
}

/* async function btnclicked(){
   

} */

let getAPI = async function(url, method = "GET", body = null) {
  try {
    const data = await fetchData(url, method, body);
    console.log(data);
    console.log(data.error_message)
    if (data.error_message == null)
    {
      window.open("../index_vragen.html");
      setCookie('id', data.id,1);
    }
    else{
      alert(data.error_message)
    }
  
  } catch (error) {
    console.log(error);
  }
};




document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    buttonclick();
});