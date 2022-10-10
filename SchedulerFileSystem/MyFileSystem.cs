using DokanNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerFileSystem
{
    public class MyFileSystem : IDokanOperations
    {
        private long freeBytesAvailable = 536_870_912;
        private long totalNumberOfBytes = 536_870_912;
        private long totalNumberOfFreeBytes = 536_870_912;
        private string path = "L:\\";

        private MyFile inputRoot;
        private MyFile outputRoot;
        private List<MyFile> inputFiles = new List<MyFile>();
        private List<MyFile> outputFiles = new List<MyFile>();
        private List<MyFile> myFiles = new List<MyFile>();
        public MyFileSystem()
        {
            string inputName = "slika";
            FileInformation inputInfo = new FileInformation()
            {
                Attributes = FileAttributes.Directory,
                CreationTime = DateTime.Now,
                Length = inputName.Length,
                FileName = "input"
            };

            string outputName = "output";
            FileInformation outputInfo = new FileInformation()
            {
                Attributes = FileAttributes.Directory,
                CreationTime = DateTime.Now,
                Length = outputName.Length,
                FileName = "output"
            };
            inputRoot = new MyFile(inputName, inputInfo);
            outputRoot = new MyFile(outputName, outputInfo);
            myFiles.Add(inputRoot);
            myFiles.Add(outputRoot);
            //MyFile test = new MyFile("L:\\input\\slika.png", new FileInformation()
            //{
            //    Attributes = FileAttributes.Normal,
            //    CreationTime = DateTime.Now,
            //    FileName = "slika.png",
            //    Length = 0
            //});
            //test.setData(new byte[0]);
            //inputFiles.Add(test);
        }
        public void Cleanup(string fileName, IDokanFileInfo info)
        {

        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
        }

        public NtStatus CreateFile(string fileName, DokanNet.FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            if (fileName == "\\")
                return NtStatus.Success;
            if (access == DokanNet.FileAccess.ReadAttributes && mode == FileMode.Open)
            {
                if (fileName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Length > 12) return NtStatus.Error;

                if (attributes == FileAttributes.Directory || info.IsDirectory)
                {
                    return NtStatus.Success;
                }
            }

            if (mode == FileMode.CreateNew)
            {

                if (fileName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Length > 12) return NtStatus.Error;

                if (attributes == FileAttributes.Directory || info.IsDirectory)
                {
                    
                }
                else
                {
                    string[] data = fileName.Split('\\');
                    MyFile newFile = new MyFile(fileName, new FileInformation()
                    {
                        FileName = data[data.Length -1],
                        Attributes = FileAttributes.Normal,
                        CreationTime = DateTime.Now,
                        Length = data[data.Length - 1].Length

                    });
                    byte[] tempArr = new byte[0];
                    newFile.setData(tempArr);
                    if(fileName.Contains("\\input"))
                    {
                        inputFiles.Add(newFile);
                        System.IO.File.Create("L:\\output\\slika.png");
                    }
                    else if(fileName.Contains("\\output"))
                    {
                        outputFiles.Add(newFile);
                    }
                }
            }
            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return DokanResult.AccessDenied;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            return DokanResult.AccessDenied;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            if (fileName == "\\")
            {
                List<FileInformation> temp = new List<FileInformation>();
                temp.Add(inputRoot.FileInfo);
                temp.Add(outputRoot.FileInfo);
                files = temp;
                return DokanResult.Success;

            }
            else if(fileName == "\\input")
            {
                files = inputFiles.Select(s => s.FileInfo).ToList();
                return DokanResult.Success;

            }
            else if (fileName == "\\output")
            {
                files = outputFiles.Select(s => s.FileInfo).ToList();
                return DokanResult.Success;

            }
            else
            {
                files = new List<FileInformation>();
                return NtStatus.Error;
            }
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams =  new List<FileInformation>();
            return NtStatus.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            return NtStatus.NotImplemented;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            freeBytesAvailable = this.freeBytesAvailable;

            totalNumberOfBytes = this.totalNumberOfBytes;
            totalNumberOfFreeBytes = this.totalNumberOfFreeBytes;
            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            if(fileName=="\\input")
            {
                fileInfo = inputRoot.FileInfo;
                return NtStatus.Success;
            }
            if(fileName=="\\output")
            {
                fileInfo = outputRoot.FileInfo;
                return NtStatus.Success;
            }

            if(inputFiles.Any(s => s.FileName.Equals(fileName)))
            {
                var file = inputFiles.Find(s => s.FileName.Equals(fileName));
                fileInfo = file.FileInfo;
                fileInfo.Length = file.getData().Length;
                return NtStatus.Success;
            }

            if(outputFiles.Any(s => s.FileName.Equals(fileName)))
            {
                var file = outputFiles.Find(s => s.FileName.Equals(fileName));
                fileInfo = file.FileInfo;
                fileInfo.Length = file.getData().Length;
                return NtStatus.Success;
            }

            fileInfo = new FileInformation() { FileName = fileName };
            return NtStatus.Error;

        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            security = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "FileSystemScheduler";
            features = FileSystemFeatures.None;
            fileSystemName = string.Empty;
            maximumComponentLength = 255;
            return DokanResult.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Mounted(IDokanFileInfo info)
        {
            //System.IO.Directory.CreateDirectory("L:\\input");
            //System.IO.Directory.CreateDirectory("L:\\output");
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            MyFile myFile;
            if(inputFiles.Any(s => s.FileName.Equals(fileName)))
            {
                myFile = inputFiles.Find(s => s.FileName.Equals(fileName));
            }
            else
            {
                myFile = outputFiles.Find(s => s.FileName.Equals(fileName));
            }

            if(myFile==null || myFile.getData()==null)
            {
                bytesRead = 0;
                return DokanResult.Success;
            }

            if (info.Context == null) // memory mapped read
            {
                using (var stream = new MemoryStream(myFile.getData()))
                {
                    //buffer = Encoding.Default.GetBytes("Hello");
                    stream.Position = offset;
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    return DokanResult.Success;
                }
            }
            else
            {
                byte[] file = myFile.getData();
                int i = 0;

                for (i = 0; i + (int)offset < file.Length && i + (int)offset < buffer.Length; i++)
                {
                    buffer[i] = file[i + offset];
                }
                bytesRead = i;
                return DokanResult.Success;
            }



        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            Console.WriteLine("--------------"+buffer.Length);
            byte[] file = new byte[buffer.Length];
            int i = 0;
            int t = 0;
            for (i = 0; i < buffer.Length; ++i, ++t)
                file[i] = buffer[i];
            bytesWritten = i;
            MyFile myFile = null;
            if(inputFiles.Any(s => s.FileName.Equals(fileName)))
            {
                myFile = inputFiles.Find(s => s.FileName.Equals(fileName));
            }
            else if(outputFiles.Any(s => s.FileName.Equals(fileName)))
            {
                myFile = outputFiles.Find(s => s.FileName.Equals(fileName));
            }

            if(myFile!=null)
            {
                
                myFile.setData(myFile.getData().Concat(file).ToArray());
                FileInformation f = myFile.FileInfo;
                f.Length = file.Length;
                myFile.FileInfo = f;
                
            }
            freeBytesAvailable -= bytesWritten;
            totalNumberOfFreeBytes -= bytesWritten;

            return DokanResult.Success;
        }
    }
}
