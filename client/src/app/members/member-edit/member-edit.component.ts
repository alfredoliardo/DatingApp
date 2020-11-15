import { Component, HostListener, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  private user: User;
  public member: Member;
  public profileForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if(this.profileForm.dirty){
      $event.returnValue = true;
    }
  }

  constructor(private accountService: AccountService, private memberService: MemberService, private toastrService: ToastrService, private fb:FormBuilder) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(): void{
    this.memberService.getMember(this.user.userName).pipe(take(1)).subscribe(
      member => {
        this.member = member;
        this.profileForm = this.fb.group({
          introduction: [this.member.introduction],
          lookingFor: [this.member.lookingFor],
          city: [this.member.city],
          country: [this.member.country],
        });

      }
    );
  }

  updateMember(): void{
    console.log(this.member);
    this.toastrService.success('Profile updated!');
    this.profileForm.reset(this.member);
  }
}
