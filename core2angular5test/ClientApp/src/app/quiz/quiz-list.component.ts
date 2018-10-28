import { Component, Inject, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {l} from "@angular/core/src/render3";

@Component({
  selector: "quiz-list",
  /*
    name of the HTML pseudo-element we'll have to use to include the component
    within another component's template; in this case, with the given value,
    it will be <quiz-list></quiz-list>
   */
  templateUrl: './quiz-list.component.html',
  styleUrls: ['./quiz-list.component.css']
})

export class QuizListComponent implements OnInit{
  @Input() class: string;
  title: string;
  selectedQuiz: Quiz;
  quizzes: Quiz[];
  http: HttpClient;
  baseUrl: string;

  constructor(http: HttpClient,
              @Inject('BASE_URL') baseUrl: string) //TODO: WTF is BASE_URL?
              //@Inject - decorator użyty by poprzez DI zainicjować parametr baseurl
  {
    this.http = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    console.log("QuizListComponent " +
      " instantiated with the following class: "
      + this.class);

    //var url = baseUrl + "api/quiz/latest/10";
    var url = this.baseUrl + "api/quiz";

    switch (this.class) {
      case "latest":
      default:
        this.title = "Latest Quizzes";
        url += "/latest/10";
        break;
      case "byTitle":
        this.title = "Quizzes By Title";
        url += "/byTitle/10";
        break;
      case "random":
        this.title = "Random Quizzes";
        url += "/random/10";
        break;
    }

    this.http.get<Quiz[]>(url).subscribe(result => {
      this.quizzes = result;
    }, error => console.error(error));
  }

  onSelect(quiz: Quiz) {
    this.selectedQuiz = quiz;
    console.log("quiz with Id "
      + this.selectedQuiz.Id,
      + " has been selected.");
  }
}
