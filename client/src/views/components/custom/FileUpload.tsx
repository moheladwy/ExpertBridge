import React, { useCallback, useEffect, useRef, useState } from "react";
import { Trash2 } from "lucide-react";
import { Controller, useController, useFormContext } from "react-hook-form";
import ReactPlayer from "react-player";
import { MediaObject } from "@/features/media/types";
import { cn } from "@/lib/util/utils";

interface FileUploadProps {
	limit: number;
	multiple: boolean;
	name: string;
	setParentMediaList: (...args: any) => any;
}

// ? FileUpload Component
const FileUpload: React.FC<FileUploadProps> = ({
	limit,
	multiple,
	name,
	setParentMediaList,
}) => {
	// ? Form Context
	const {
		control,
		formState: { isSubmitting, errors },
	} = useFormContext();

	// ? State with useState()
	const { field } = useController({ name, control });
	const [singleFile, setSingleFile] = useState<File[]>([]);
	const [fileList, setFileList] = useState<File[]>([]);
	const [fileUrls, setFileUrls] = useState<string[]>([]);
	const wrapperRef = useRef<HTMLDivElement>(null);

	// ? Toggle the dragover class
	const onDragEnter = () => wrapperRef.current?.classList.add("dragover");
	const onDragLeave = () => wrapperRef.current?.classList.remove("dragover");

	// ? Image Upload Service
	const onFileDrop = useCallback(
		(e: React.SyntheticEvent<EventTarget>) => {
			const target = e.target as HTMLInputElement;
			if (!target.files) return;

			if (limit === 1) {
				const newFile = Object.values(target.files).map(
					(file: File) => file
				);
				if (singleFile.length >= 1)
					return alert("Only a single image allowed");
				setSingleFile(newFile);
				field.onChange(newFile[0]);
			}

			if (multiple) {
				const newFiles = Object.values(target.files).map(
					(file: File) => file
				);
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
		const urls = (multiple ? fileList : singleFile).map((file) =>
			URL.createObjectURL(file)
		);

		const mediaList: MediaObject[] = (multiple ? fileList : singleFile).map(
			(file) => ({
				url: URL.createObjectURL(file),
				file: file,
				// type: getType(file),
				type: file.type,
			})
		);

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
			<div className="flex">
				<div className="bg-white rounded-4xl shadow-[rgba(149,157,165,0.2)_0px_8px_24px] p-4 hover:opacity-60 transition-opacity">
					<div
						className="flex justify-center items-center relative w-full h-[25vh] border-2 border-dashed border-[#4267b2] rounded-[20px] p-5"
						ref={wrapperRef}
						onDragEnter={onDragEnter}
						onDragLeave={onDragLeave}
						onDrop={onDragLeave}
					>
						<div className="flex justify-center p-1 text-center">
							<div className="font-bold max-md:text-sm max-md:w-full">
								{limit > 1
									? "Browse to upload files"
									: "Browse to upload file"}
							</div>

							<div className="max-md:hidden">
								<div className="text-slate-600 max-md:text-sm">
									Supported Files
								</div>
								<div className="text-slate-600 max-md:text-sm">
									JPG, JPEG, PNG
								</div>
							</div>
						</div>
						<Controller
							name={name}
							defaultValue=""
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
					</div>
				</div>

				<div
					className={cn(
						"text-center my-1",
						errors[name] && "text-red-500"
					)}
				>
					{(errors[name] ? errors[name].message : "")?.toString()}
				</div>

				{(fileList.length > 0 || singleFile.length > 0) && (
					<div className="ml-3">
						<div className="space-y-2 my-2">
							{(multiple ? fileList : singleFile).map(
								(item, index) => {
									const fileType = item.type.split("/")[0]; // 'image' or 'video'
									const isImage = fileType === "image";
									const isVideo = fileType === "video";

									return (
										<div
											key={index}
											className="relative bg-[#f5f8ff] rounded-xl p-2"
										>
											<div className="flex items-center justify-between w-full">
												<div className="flex">
													{isImage ? (
														<img
															src={
																fileUrls[index]
															}
															alt="upload"
															style={{
																height: "3.5rem",
																objectFit:
																	"contain",
															}}
														/>
													) : isVideo ? (
														<ReactPlayer
															url={
																fileUrls[index]
															}
															width="100px"
															height="56px"
															controls
														/>
													) : null}

													<div className="mx-4 w-2/4">
														<div className="text-sm max-xl:hidden">
															{item.name}
														</div>
														<div className="text-sm max-xl:hidden">
															{calcSize(
																item.size
															)}
														</div>
													</div>
												</div>

												<button
													onClick={() => {
														if (multiple) {
															fileRemove(item);
														} else {
															fileSingleRemove();
														}
													}}
													className="max-lg:w-7 max-md:w-4 text-[#df2c0e] hover:text-red-700 transition-colors"
												>
													<Trash2 className="max-lg:w-5" />
												</button>
											</div>
										</div>
									);
								}
							)}
						</div>
					</div>
				)}
			</div>
		</>
	);
};

export default FileUpload;
