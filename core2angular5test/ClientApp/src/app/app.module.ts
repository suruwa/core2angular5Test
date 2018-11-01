import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { QuizListComponent} from "./quiz/quiz-list.component";
import { QuizComponent} from "./quiz/quiz.component";
import { PageNotFoundComponent } from "./pagenotfound/pagenotfound.component";
import { AboutComponent} from "./about/about.component";
import { LoginComponent } from "./login/login.component";
import { QuizEditComponent } from "./quiz/quiz-edit.component";
import { QuestionListComponent } from "./question/question-list.component";
import { QuestionEditComponent } from "./question/question-edit.component";
import { AnswerListComponent } from "./answer/answer-list.component";
import { AnswerEditComponent } from "./answer/answer-edit.component";
import { ResultListComponent } from "./result/result-list.component";

//Routing
//PathLocationStrategy - nowa technika, używa history.pushstate
//HashLocationStrategy - stare przeglądarki, po hashu

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        QuizListComponent,
        QuizComponent,
        QuizEditComponent,
        LoginComponent,
        PageNotFoundComponent,
        AboutComponent,
        QuestionListComponent,
        QuestionEditComponent,
        AnswerListComponent,
        AnswerEditComponent,
        ResultListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
        { path: '', component: HomeComponent, pathMatch: 'full' },
        { path: 'home', component: HomeComponent },
        { path: 'quiz/create', component: QuizEditComponent },//przed :id koniecznie
        { path: 'quiz/edit/:id', component: QuizEditComponent },
        { path: 'quiz/:id', component: QuizComponent },
        { path: 'question/create/:id', component: QuestionEditComponent },
        { path: 'question/edit/:id', component: QuestionEditComponent },
        { path: 'answer/edit/:id', component: AnswerEditComponent },
        { path: 'answer/create/:id', component: AnswerEditComponent },
        { path: 'about', component: AboutComponent },
        { path: 'login', component: LoginComponent },
        { path: '**', component: PageNotFoundComponent }

      //{ path: '', redirectTo: 'home', pathMatch: 'full' },
      //{ path: 'home', component: HomeComponent },
      //{ path: '**', redirectTo: 'home' }//global fallback
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }


