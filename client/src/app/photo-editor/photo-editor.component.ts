import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { Photo } from '../_models/Photo';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { MemberService } from '../_services/member.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member;
  uploader:FileUploader;
  hasBaseDropZoneOver:boolean = false;
  baseUrl:string = environment.apiUrl;
  response:string;
  user:User;
 
  constructor(private accountService:AccountService, private memberService:MemberService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    });
  }

  ngOnInit(): void {
    this.InitializeFileUploader();
  }

  private InitializeFileUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    };
  }

  deletePhoto(photo:Photo){
    this.memberService.deletePhoto(photo.id).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id != photo.id);
    });
  }


  public setMainPhoto(photo:Photo){
   this.memberService.setMainPhoto(photo.id).subscribe(() => {
     this.user.photoUrl = photo.url;
     this.accountService.setCurrentUser(this.user);
     this.member.photoUrl = photo.url;
     this.member.photos.forEach((p) => {
       if(p.isMain) p.isMain = false;
       if(p.id === photo.id) p.isMain = true;
     });
   }); 
  }

  public fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }
 
  public fileOverAnother(e:any):void {
    
  }
}
