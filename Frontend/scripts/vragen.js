let datavragen = [];
let dataantwoord = [];
let domvragen, domantwoord;

const getDomElements = function(){
    console.log("dom geladen");
    let getidquiz, getidgame;
    getidquiz = localStorage.getItem('quizid');
    getidgame = localStorage.getItem('gameid');
    getAPI("bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6")
   
    
};


const uitlezendata = () => {
	handleData('.json', processdata);
};

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

const vragenophalen = function(){
    let vraag = "";
    vraag += `  <div class="js-vragen c-img__center">
        <h3 class="c-zero_margin c-color_white">${datavragen[3].question}</h3>
     </div>  `;
    
    domvragen.innerHTML = vraag;
    let answersarr = datavragen[3].answers;
    for (let i = 0; i < answersarr.length; i++){
        const quizantwoord = answersarr[i];
        
        
        let antwoord = {};
        if (quizantwoord)
        {
            antwoord['answer'] = quizantwoord.answer;
            antwoord['correct'] = quizantwoord.correct;
        }
        dataantwoord.push(antwoord);
        
    }

    let antwoord = "";
    console.log(dataantwoord[0].correct);
    antwoord += `<div id="card-a" class="c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">A: ${dataantwoord[0].answer}</h4>
    </div>
</div>
<div id="card-b" class="c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">B:  ${dataantwoord[1].answer}</h4>
    </div>
</div>
<div id="card-c" class="c-card-c c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">C:  ${dataantwoord[2].answer}</h4>                    
    </div>
</div>
<div id="card-d" class="c-card-d c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">D:  ${dataantwoord[3].answer}</h4>
    </div>
</div>`

    
    domantwoord.innerHTML = antwoord;
    
}


const makeymakey = () => {
	document.addEventListener('keyup', function(key) {
        let divcorrect = document.getElementById('card-a');
		//keyup omdat je geen probleem zou hebben met het lan inihouden van een bepaalde toets
		switch (key.keyCode) {
			case 37:
                console.log('left');
                /* divcorrect = document.getElementById('card-a'); */
                divcorrect.style.backgroundColor = 'green';
				break;
			case 38:
                console.log('up');
                let divfalseb = document.getElementById('card-b');
                divfalseb.style.backgroundColor = 'red';
                divcorrect.style.border = 'thick solid green';
                break;
			case 39:
                console.log('right');
                let divfalsec = document.getElementById('card-c');
                divfalsec.style.backgroundColor = 'red';
                divcorrect.style.border = 'thick solid green';
				break;
			case 40:
                console.log('down');
                let divfalsed = document.getElementById('card-d');
                divfalsed.style.backgroundColor = 'red';
                divcorrect.style.border = 'thick solid green';
				break;
			case 13:
				console.log('enter');
				break;
		}
	});
};


document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    domvragen = document.querySelector('.js-vragen')
    domantwoord = document.querySelector('.js-antwoord')
    makeymakey();
});
