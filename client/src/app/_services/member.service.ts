import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  loadMembers(){
    return this.http.get<Member[]>(this.baseUrl+'users')
  }

  getMember(memberName:string){
    return this.http.get<Member>(this.baseUrl+'users'+'/'+memberName)
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member);
  }
}
