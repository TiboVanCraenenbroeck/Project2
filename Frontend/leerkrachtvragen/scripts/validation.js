
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

async function btnclicked(){
    let mailvalue = document.getElementById('username').value;
    console.log(mailvalue);
    let ww = document.getElementById('password').value;
   
    getAPI(`login/${mailvalue}/${ww}`)
}

let getAPI = async function(url, method = "GET", body = null) {
  try {
    const data = await fetchData(url, method, body);
    console.log(data);
  } catch (error) {
    console.log(error);
  }
};




document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
});