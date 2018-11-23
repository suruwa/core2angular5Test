import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AuthService } from "./services/auth.service";
import { AuthInterceptor } from "./services/auth.interceptor";
import { AuthResponseInterceptor } from "./services/auth.response.interceptor";

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
import { QuizSearchComponent} from "./quiz/quiz-search.component";
import { ResultEditComponent } from "./result/result-edit.component";
import { RegisterComponent } from "./user/register.component";

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
        ResultListComponent,
        QuizSearchComponent,
        ResultEditComponent,
        RegisterComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
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
            { path: 'result/edit/:id', component: ResultEditComponent },
            { path: 'result/create/:id', component: ResultEditComponent },
            { path: 'about', component: AboutComponent },
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: '**', component: PageNotFoundComponent }

          //{ path: '', redirectTo: 'home', pathMatch: 'full' },
          //{ path: 'home', component: HomeComponent },
          //{ path: '**', redirectTo: 'home' }//global fallback
        ])
    ],
    providers: [
        AuthService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthResponseInterceptor,
            multi: true
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }


