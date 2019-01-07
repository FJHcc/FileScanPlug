package file;

import java.io.File;
import java.io.FileFilter;
import java.util.ArrayList;
import java.util.List;

//查找目录下指定文件名的文件
public class FileSearch {

    public static File[] searchFile(File folder, final String keyWord) {

        File[] subFolders = folder.listFiles(new FileFilter() {
            //accept方法
            @Override
            public boolean accept(File pathname) {
                if (pathname.isDirectory()
                        || (pathname.isFile() && pathname.getName().toLowerCase().contains(keyWord.toLowerCase())))// 目录或文件包含关键字
                    return true;
                return false;
            }
        });
        List<File> result = new ArrayList<File>();// 声明一个集合
        for (int i = 0; i < subFolders.length; i++) {// 循环显示文件夹或文件
            if (subFolders[i].isFile()) {// 如果是文件则将文件添加到结果列表中
                result.add(subFolders[i]);
            } else {// 如果是文件夹，则递归调用本方法，然后把所有的文件加到结果列表中
                File[] foldResult = searchFile(subFolders[i], keyWord);
                for (int j = 0; j < foldResult.length; j++) {
                    result.add(foldResult[j]);// 文件保存到集合中
                }
            }
        }

        File files[] = new File[result.size()];// 声明文件数组，长度为集合的长度
        result.toArray(files);// 集合数组化
        return files;
    }
}
