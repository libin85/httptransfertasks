﻿using System;
using Foundation;


namespace Plugin.HttpTransferTasks
{
    public class HttpTask : AbstractHttpTask, IIosHttpTask
    {
        readonly NSUrlSessionTask task;


        public HttpTask(TaskConfiguration config, NSUrlSessionTask task) : base(config, task is NSUrlSessionUploadTask)
        {
            this.task = task;
            this.Identifier = task.TaskIdentifier.ToString();
        }


        public override void Start()
        {
            this.task.Resume();
        }


        public override void Pause()
        {
            this.Status = TaskStatus.Paused;
            this.task.Suspend();
        }


        public override void Cancel()
        {
            this.Status = TaskStatus.Cancelled;
            this.task.Cancel();
        }


        public void SetDownloadComplete(string tempLocation)
        {
            this.LocalFilePath = tempLocation;
            this.Status = TaskStatus.Completed;
        }


        public void SetResumeOffset(long resumeOffset, long expectedTotalBytes)
        {
            this.ResumeOffset = resumeOffset;
            this.BytesTransferred = resumeOffset;
            this.Status = TaskStatus.Resumed;

			this.RunCalculations();
		}


        public void SetData(long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
        {
            this.RemoteFileName = this.task.Response?.SuggestedFilename;
            this.Status = TaskStatus.Running;
            this.BytesTransferred = totalBytesWritten;
            this.FileSize = totalBytesExpectedToWrite;

            this.RunCalculations();
        }


        public void SetStatus(TaskStatus status) => this.Status = status;
        public void SetError(NSError error) => this.Exception = new Exception(error.LocalizedDescription);
    }
}