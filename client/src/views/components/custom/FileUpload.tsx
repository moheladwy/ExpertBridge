import {
  Box,
  FormHelperText,
  IconButton,
  Stack,
  Typography,
} from '@mui/material';
import { styled } from '@mui/material/styles';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import DeleteIcon from '@mui/icons-material/Delete';
import uploadImg from '@/assets/default-avatar.png';
import { Controller, useController, useFormContext } from 'react-hook-form';
import ReactPlayer from 'react-player';
import { MediaObject } from '@/features/media/types';

interface FileUploadProps {
  limit: number;
  multiple: boolean;
  name: string;
  setParentMediaList: (...args: any) => any;
}

const CustomBox = styled(Box)({
  '&.MuiBox-root': {
    backgroundColor: '#fff',
    borderRadius: '2rem',
    boxShadow: 'rgba(149, 157, 165, 0.2) 0px 8px 24px',
    padding: '1rem',
  },
  '&.MuiBox-root:hover, &.MuiBox-root.dragover': {
    opacity: 0.6,
  },
});


// ? FileUpload Component
const FileUpload: React.FC<FileUploadProps> = ({ limit, multiple, name, setParentMediaList }) => {
  // ? Form Context
  const {
    control,
    formState: { isSubmitting, errors },
  } = useFormContext();

  const getType = (file: File): 'image' | 'video' | 'pdf' => {
    if (file.type.startsWith('image')) return 'image';
    if (file.type.startsWith('video')) return 'video';
    if (file.type === 'application/pdf') return 'pdf';
    throw new Error('Unsupported file type');
  };

  // ? State with useState()
  const { field } = useController({ name, control });
  const [singleFile, setSingleFile] = useState<File[]>([]);
  const [fileList, setFileList] = useState<File[]>([]);
  const [fileUrls, setFileUrls] = useState<string[]>([]);
  const wrapperRef = useRef<HTMLDivElement>(null);

  // ? Toggle the dragover class
  const onDragEnter = () => wrapperRef.current?.classList.add('dragover');
  const onDragLeave = () => wrapperRef.current?.classList.remove('dragover');

  // ? Image Upload Service
  const onFileDrop = useCallback(
    (e: React.SyntheticEvent<EventTarget>) => {
      const target = e.target as HTMLInputElement;
      if (!target.files) return;

      if (limit === 1) {
        const newFile = Object.values(target.files).map((file: File) => file);
        if (singleFile.length >= 1) return alert('Only a single image allowed');
        setSingleFile(newFile);
        field.onChange(newFile[0]);
      }

      if (multiple) {
        const newFiles = Object.values(target.files).map((file: File) => file);
        if (newFiles) {
          const updatedList = [...fileList, ...newFiles];
          if (updatedList.length > limit || newFiles.length > 3) {
            return alert(`Image must not be more than ${limit}`);
          }
          setFileList(updatedList);
          field.onChange(updatedList);
        }
      }
    },
    [field, fileList, limit, multiple, singleFile]
  );

  useEffect(() => {
    const urls = (multiple ? fileList : singleFile).map(file => URL.createObjectURL(file));

    const mediaList: MediaObject[] = (multiple ? fileList : singleFile).map(file => ({
      url: URL.createObjectURL(file),
      file: file,
      type: getType(file),
    }));

    setParentMediaList(mediaList);
    setFileUrls(urls);
  }, [singleFile, fileList, multiple, setParentMediaList]);

  // ? remove multiple images
  const fileRemove = (file: File) => {
    const updatedList = [...fileList];
    updatedList.splice(fileList.indexOf(file), 1);
    setFileList(updatedList);
  };

  // ? remove single image
  const fileSingleRemove = () => {
    setSingleFile([]);
  };

  // ? TypeScript Type
  type CustomType = 'jpg' | 'png' | 'svg' | 'mp4' | 'mkv';

  // ? Calculate Size in KiloByte and MegaByte
  const calcSize = (size: number) => {
    return size < 1000000
      ? `${Math.floor(size / 1000)} KB`
      : `${Math.floor(size / 1000000)} MB`;
  };

  // ? Reset the State
  useEffect(() => {
    if (isSubmitting) {
      setFileList([]);
      setSingleFile([]);
    }
  }, [isSubmitting]);


  // ? Actual JSX
  return (
    <>
      <CustomBox>
        <Box
          display='flex'
          justifyContent='center'
          alignItems='center'
          sx={{
            position: 'relative',
            width: '100%',
            height: '13rem',
            border: '2px dashed #4267b2',
            borderRadius: '20px',
          }}
          ref={wrapperRef}
          onDragEnter={onDragEnter}
          onDragLeave={onDragLeave}
          onDrop={onDragLeave}
        >
          <Stack justifyContent='center' sx={{ p: 1, textAlign: 'center' }}>
            <Typography sx={{ color: '#ccc' }}>
              {limit > 1 ? 'Browse files to upload' : 'Browse file to upload'}
            </Typography>
            <div>
              <img
                src={uploadImg}
                alt='file upload'
                style={{ width: '5rem' }}
              />
            </div>
            <Typography variant='body1' component='span'>
              <strong>Supported Files</strong>
            </Typography>
            <Typography variant='body2' component='span'>
              JPG, JPEG, PNG
            </Typography>
          </Stack>
          <Controller
            name={name}
            defaultValue=''
            control={control}
            render={({ field: { name, onBlur, ref } }) => (
              <input
                type="file"
                name={name}
                onBlur={onBlur}
                ref={ref}
                onChange={onFileDrop}
                multiple={multiple}
                accept="image/jpg, image/jpeg, image/png, video/mp4, video/webm, video/ogg"
                style={{
                  opacity: 0,
                  position: "absolute",
                  top: 0,
                  left: 0,
                  width: "100%",
                  height: "100%",
                  cursor: "pointer",
                }}
              />

            )}
          />
        </Box>
      </CustomBox>

      <FormHelperText
        sx={{ textAlign: 'center', my: 1 }}
        error={!!errors[name]}
      >
        {
          (errors[name] ? errors[name].message : '')?.toString()
        }
      </FormHelperText>

      import ReactPlayer from 'react-player';

      {(fileList.length > 0 || singleFile.length > 0) && (
        <Stack spacing={2} sx={{ my: 2 }}>
          {(multiple ? fileList : singleFile).map((item, index) => {
            const fileType = item.type.split('/')[0]; // 'image' or 'video'
            const isImage = fileType === 'image';
            const isVideo = fileType === 'video';

            return (
              <Box
                key={index}
                sx={{
                  position: 'relative',
                  backgroundColor: '#f5f8ff',
                  borderRadius: 1.5,
                  p: 0.5,
                }}
              >
                <Box display='flex'>
                  {isImage ? (
                    <img
                      src={fileUrls[index]}
                      alt='upload'
                      style={{
                        height: '3.5rem',
                        objectFit: 'contain',
                      }}
                    />
                  ) : isVideo ? (
                    <ReactPlayer
                      url={fileUrls[index]}
                      width='100px'
                      height='56px'
                      controls
                    />
                  ) : null}

                  <Box sx={{ ml: 1 }}>
                    <Typography>{item.name}</Typography>
                    <Typography variant='body2'>
                      {calcSize(item.size)}
                    </Typography>
                  </Box>
                </Box>

                <IconButton
                  onClick={() => {
                    if (multiple) {
                      fileRemove(item);
                    } else {
                      fileSingleRemove();
                    }
                  }}
                  sx={{
                    color: '#df2c0e',
                    position: 'absolute',
                    right: '1rem',
                    top: '50%',
                    transform: 'translateY(-50%)',
                  }}
                >
                  <DeleteIcon />
                </IconButton>
              </Box>
            );
          })}
        </Stack>
      )}

    </>
  );
};

export default FileUpload;
