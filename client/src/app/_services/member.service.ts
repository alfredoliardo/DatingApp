import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl;
  members:Member[] = [];

  constructor(private http: HttpClient) { }

  loadMembers(){
    if(this.members.length > 0){
      return of(this.members);
    }
    return this.http.get<Member[]>(this.baseUrl+'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(memberName:string){
    var member = this.members.find(m => m.userName === memberName);
    if(member !== undefined){
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl+'users'+'/'+memberName)
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        var index = this.members.indexOf(member);
        this.members[index];
      })
    );
  }

  setMainPhoto(photoId:number){
      return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}
