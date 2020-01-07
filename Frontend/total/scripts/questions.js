let id; 
let decid;
let uri = "https://mctproject2.azurewebsites.net/api/v1/subject?cookie_id=";


const getDomElements = function(){
  console.log("dom geladen");
  id = getCookie('id');
  decid = id.replace("+", "%2B");
  console.log(decid);  
};

function postdata(url = '', data= {})
{
  console.log('ik zit hier')
  fetch(url, {
    method: 'POST', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}

const buttonclick = function(){
  let btn = document.querySelector('.js-btn-onderwerp-verzenden');
  btn.addEventListener('click', function()
  {
    var subjectinput = document.getElementById("js-btn-onderwerp-verzenden");
    let valuesubject = subjectinput.value
    let data = {title: valuesubject, description: ''}

    id = getCookie('id');
    console.log(id);
    decid = id.replace("+", "%2B");
    console.log(decid);
    postdata(uri+decid, data)
    document.getElementById("js-btn-onderwerp-verzenden").value = "";
  })
}

const buttonclickvragen = function(){
  let btnvragen = document.querySelector('.js-btn-verzenden');
  btnvragen.addEventListener('click', function(){
    let vraag = document.getElementById('js-vraag').value;
    var antwoorda = document.getElementById('js-antwoorda').value;
    var antwoordb = document.getElementById('js-antwoordb').value;
    var antwoordc = document.getElementById('js-antwoordc').value;
    var antwoordd = document.getElementById('js-antwoordd').value;
    var radiobtna = document.getElementById('js-rbtn-a').checked;
    var radiobtnb = document.getElementById('js-rbtn-b').checked;
    var radiobtnc = document.getElementById('js-rbtn-c').checked;
    var radiobtnd = document.getElementById('js-rbtn-d').checked; 
    var level = document.getElementById('js-level').value;

    let data = {
      question: vraag, 
      difficult: level,
      answers: [
        {
        answer: antwoorda,
        correct: radiobtna
        },
        {
        answer: antwoordb,
        correct: radiobtnb
        },
        {
        answer: antwoordc,
        correct: radiobtnc
        },
        {
        answer: antwoordd,
        correct: radiobtnd
        }
      ]  
    }

    console.log(data);
    /* let onderwerpid = "bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6" */
    let apionderwerp = "https://mctproject2.azurewebsites.net/api/v1/question/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id="
    id = getCookie('id');
    console.log(id);
    decid = id.replace("+", "%2B");
    console.log(decid);
    postdata(apionderwerp+decid,data)

  })
}


document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    buttonclick();
    buttonclickvragen();
});