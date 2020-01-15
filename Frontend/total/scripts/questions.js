let id; 
let decid;
let uri = "https://mctproject2.azurewebsites.net/api/v1/subject?cookie_id=";
let domonderwerpen;
let quiz_ids = [];

const getDomElements = function(){
  console.log("dom geladen");
  id = getCookie('id');
  decid = id.replace("+", "%2B");
};

function postdata(url = '', data= {})
{
  fetch(url, {
    method: 'POST', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
    /* location.reload(); */
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}

const buttonclick = function(){
  let btn = document.querySelector('.js-btn-onderwerp-verzenden');
  btn.addEventListener('click', function()
  {
    var subjectinput = document.getElementById("js-input-onderwerp");
    let valuesubject = subjectinput.value
    let data = {title: valuesubject, description: ''}

    id = getCookie('id');
    decid = id.replace("+", "%2B");
    postdata(uri+decid, data)
    document.getElementById("js-input-onderwerp").value = "";

   /*  console.log(valuesubject); */
    /* console.log(quiz_ids); */
   /*  for (var quiz_id in quiz_ids)
    {
      if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
      const everything = quiz_ids[quiz_id];
      const qid = everything.quiz_id;
      const title = everything.title;
      if (title == valuesubject)
      {
        console.log(qid);
        localStorage.setItem('quizid', qid);
      }
    } */
   
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
    var levelnr = Number(level);

    let data = {
      question: vraag, 
      difficult: levelnr,
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
    var subjectinput = document.getElementById("js-input-onderwerp");
    let valuesubject = subjectinput.value
    for (var quiz_id in quiz_ids)
    {
      if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
      const everything = quiz_ids[quiz_id];
      const qid = everything.quiz_id;
      const title = everything.title;
      if (title == valuesubject)
      {
        console.log(qid);
        localStorage.setItem('quizid', qid);
      }
    }

    let onderwerpid = "bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6"
    let apio = "https://mctproject2.azurewebsites.net/api/v1/question/"+onderwerpid+"?cookie_id=";
   /*  let apionderwerp = "https://mctproject2.azurewebsites.net/api/v1/question/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id=" */
    id = getCookie('id');
    decid = id.replace("+", "%2B");
    
    postdata(apio+decid,data)

    

  })
}

//get onderwerpen

const getonderwerpen = function(){
  handleData(`https://mctproject2.azurewebsites.net/api/v1/subjects`, showonderwerpen)
}

const showonderwerpen = function(data)
{
  
  domonderwerpen = document.querySelector('.js-selecteditem');
  let arronderwerpen = data
  let OnderwerpHTML = ``;
  
  for (let i = 0; i < data.length; i++)
  {
    let ids = {};
    const quizdata = data[i];
    if(quizdata.quiz_id){
      ids['quiz_id'] = quizdata.quiz_id;
    }
    if (quizdata.title)
    {
      ids['title'] = quizdata.title;
    }
    
    quiz_ids.push(ids);

    OnderwerpHTML += `
    <datalist class="js-selecteditem" id="onderwerpen">
                  <option value=${data[i].title}>
    </datalist>
    `;
   
  }

  domonderwerpen.innerHTML = OnderwerpHTML;
}

//get vragen

const fetchData = async function(url, method = "GET", body = null) {
  return fetch(`https://mctproject2.azurewebsites.net/api/v1/questions/${url}`, {
    method: method,
    body: body,
    headers: { "content-type": "application/json" }
  })
    .then(r => r.json())
    .then(data => data);
};

let getAPI = async function(url, method = "GET", body = null) {
    try {
      const data = await fetchData(url, method, body);
      getrandomnr(0, data.length);
     /*  console.log(data.error_message) */
      for (let i = 0; i < data.length; i++)
      {
          let vragen = {};
          const quizvragen = data[i];
          if (quizvragen)
          {
              vragen['question'] = quizvragen.question;
              vragen['answers'] = quizvragen.answers;
          } 
          datavragen.push(vragen);
       
      }

      vragenophalen();


      
    } catch (error) {
      console.log(error);
    }
  };


document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    buttonclick();
    buttonclickvragen();
    getonderwerpen();
    getAPI("bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6");
});